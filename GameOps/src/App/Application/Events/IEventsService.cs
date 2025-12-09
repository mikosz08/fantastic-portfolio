using App.Contracts;

namespace App.Application.Events;

public interface IEventsService
{
    Task<EventWriteResult> CreateAsync(EventCreateDto dto, string idempotencyKey, CancellationToken ct = default);
}

public sealed record EventWriteResult(EventReadDto Event, bool IsReplay);
