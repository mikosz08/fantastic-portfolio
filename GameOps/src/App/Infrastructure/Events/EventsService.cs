using App.Application.Events;
using App.Contracts;
using App.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace App.Infrastructure.Events;

public sealed class EventsService : IEventsService
{
    private readonly AppDbContext _db;
    private readonly ILogger<EventsService> _logger;

    public EventsService(AppDbContext db, ILogger<EventsService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<EventWriteResult> CreateAsync(EventCreateDto dto, string idempotencyKey, CancellationToken ct = default)
    {
        var entity = dto.ToEntity(idempotencyKey);

        try
        {
            _db.Events.Add(entity);
            await _db.SaveChangesAsync(ct);
            return new EventWriteResult(entity.ToReadDto(), IsReplay: false);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            _db.Entry(entity).State = EntityState.Detached;

            var existing = await _db.Events
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.IdempotencyKey == idempotencyKey, ct);

            if (existing is null)
            {
                _logger.LogWarning("Unique violation but row missing for key {Key}", idempotencyKey);
                throw;
            }

            return new EventWriteResult(existing.ToReadDto(), IsReplay: true);
        }

    }

    private static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation;

}
