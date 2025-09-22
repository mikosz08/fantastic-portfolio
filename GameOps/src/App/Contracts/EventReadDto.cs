using System.Text.Json;

namespace App.Contracts;

public sealed class EventReadDto
{
    public required Guid Id { get; init; }
    public required string Type { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public string? PlayerId { get; init; }
    public string? SessionId { get; init; }
    public JsonElement? Payload { get; init; }
    public required DateTimeOffset IngestedAt { get; init; }
}