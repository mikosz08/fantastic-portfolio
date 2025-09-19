using System;
using System.Text.Json;

namespace App.Data;

public sealed class Event
{
    public Guid Id { get; set; }                                // primary key
    public DateTimeOffset OccurredAt { get; set; }              // when it happened (domain time)
    public string Type { get; set; } = string.Empty;            // event type/name
    public string? PlayerId { get; set; }                       // optional
    public string? SessionId { get; set; }                      // optional
    public string IdempotencyKey { get; set; } = string.Empty;  // unique per ingest attempt
    public JsonDocument? Payload { get; set; }                  // raw payload (jsonb)
    public DateTimeOffset IngestedAt { get; set; }              // server time when stored
}
