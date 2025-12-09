using App.Data;
using App.Endpoints;
using App.Application.Events;
using App.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  "ConnectionStrings:Default" "Host=...;Username=...;Password=...;Database=..."
var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
        npgsql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(2), errorCodesToAdd: null);
    });
});

builder.Services.AddScoped<IEventsService, EventsService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddSimpleConsole();

var app = builder.Build();

app.Logger.LogInformation("Hello World! Environment: {env}",
    app.Environment.EnvironmentName);


// DEV
if (app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        app.Logger.LogWarning("\u001b[33mDatabase Migrations starting...\u001b[33m");
        db.Database.Migrate();
        app.Logger.LogWarning("\u001b[33mDatabase Migrations completed successfully.\u001b[33m");
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning("\u001b[33mDatabase connection unavailable; migrations skipped. Reason: {Reason}\u001b[33m", ex.Message);
    }

    app.UseSwagger();
    app.UseSwaggerUI();

}

app.MapEvents();

app.MapGet("/", async (AppDbContext db, IWebHostEnvironment env) =>
{
    bool dbOk;
    try
    {
        dbOk = await db.Database.CanConnectAsync();
    }
    catch
    {
        dbOk = false;
    }

    var version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

    var payload = new
    {
        message = "Welcome to GameOps API",
        environment = env.EnvironmentName,
        serverTimeUtc = DateTimeOffset.UtcNow,
        version,
        db = new { ok = dbOk },
        tips = new[]
        {
            "POST /events with JSON body to submit an event",
            "Check /swagger for interactive API docs",
            "Ping /readyz to verify readiness"
        },
        links = new
        {
            swagger = "/swagger",
            readiness = "/readyz",
            events = "/events"
        }
    };

    return Results.Ok(payload);
});

app.MapGet("/readyz", () => Results.Ok(new { status = "ok" }));


app.Run();

public partial class Program { }
