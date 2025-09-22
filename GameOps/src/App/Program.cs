using App.Data;
using App.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration["ConnectionStrings:Default"] ??
    builder.Configuration["ConnectionStrings__Default"] ??
    "Host=localhost;Username=app;Password=app;Database=gameops";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsql =>
    {
        // keep migrations in the App assembly
        npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
        // transient-retry for startup race with DB
        npgsql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(2), errorCodesToAdd: null);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// DEV
if (app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEvents();

app.MapGet("/readyz", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program { }
