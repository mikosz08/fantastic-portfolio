using App.Contracts;
using App.Http;


namespace App.Endpoints;

public static class EventsEndpoints
{
    public static IEndpointRouteBuilder MapEvents(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/events")
                          .WithTags("Events");

        group.MapPost("", (HttpRequest req, EventCreateDto dto) =>
        {
            if (!IdempotencyKey.TryGet(req, out var key))
                return ProblemFactory.BadRequest("Missing Idempotency-Key header");

            var errors = new Dictionary<string, string[]>();
            if (string.IsNullOrWhiteSpace(dto.Type))
                errors[nameof(dto.Type)] = new[] { "Type is required." };
            if (dto.OccurredAt == default)
                errors[nameof(dto.OccurredAt)] = new[] { "OccurredAt must be a valid timestamp." };

            if (errors.Count > 0)
                return ProblemFactory.BadRequest("Validation failed", errors);

            // TEMP for M0-4-1: echo request; real persistence in M0-4-2
            return Results.Accepted(
                uri: null,
                value: new { request = dto, idempotencyKey = key }
            );
        })
        .WithName("PostEvents")
        .ProducesValidationProblem()
        .WithOpenApi(); // optional

        return routes;
    }
}
