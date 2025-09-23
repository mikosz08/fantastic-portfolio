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
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogWarning("\u001b[33mDatabase Migrations starting...\u001b[33m");
        db.Database.Migrate();
        logger.LogWarning("\u001b[33mDatabase Migrations completed successfully.\u001b[33m");
    }
    catch (Exception ex)
    {
        logger.LogWarning("\u001b[33mDatabase connection unavailable; migrations skipped. Reason: \u001b[33m{Reason}[0m", ex.Message);
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEvents();

app.MapGet("/readyz", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program { }
