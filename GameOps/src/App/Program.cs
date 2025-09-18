using App.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Prefer env var: ConnectionStrings__Default (Compose), fallback to appsettings
var cs =
    builder.Configuration["ConnectionStrings:Default"] ??
    builder.Configuration["ConnectionStrings__Default"] ??
    "Host=localhost;Username=app;Password=app;Database=gameops"; // dev fallback

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(cs, npgsql =>
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

// DEV-only: apply pending migrations on start (handy for local runs)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// simple readiness check (will be enhanced later)
app.MapGet("/readyz", () => Results.Ok(new { status = "ok" }));

app.Run();
