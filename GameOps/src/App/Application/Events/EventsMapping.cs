using System.Text.Json;
using App.Contracts;
using App.Data;

namespace App.Application.Events;

public static class EventMappings
{
    public static Event ToEntity(this EventCreateDto dto, string idempotencyKey)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentException.ThrowIfNullOrWhiteSpace(idempotencyKey);

        return new Event
        {
            Id = Guid.NewGuid(),
            OccurredAt = dto.OccurredAt,
            Type = dto.Type.Trim(),
            PlayerId = dto.PlayerId,
            SessionId = dto.SessionId,
            IdempotencyKey = idempotencyKey.Trim(),
            Payload = dto.Payload.HasValue ? JsonDocument.Parse(dto.Payload.Value.GetRawText()) : null
        };
    }

    public static EventReadDto ToReadDto(this Event entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new EventReadDto
        {
            Id = entity.Id,
            Type = entity.Type,
            OccurredAt = entity.OccurredAt,
            PlayerId = entity.PlayerId,
            SessionId = entity.SessionId,
            Payload = entity.Payload?.RootElement.Clone(),
            IngestedAt = entity.IngestedAt
        };
    }
}
