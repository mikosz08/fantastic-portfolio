using App.Application.Events;
using App.Contracts;
using App.Http;


namespace App.Endpoints;

public static class EventsEndpoints
{
    public static IEndpointRouteBuilder MapEvents(this IEndpointRouteBuilder routes)
    {

        var group = routes.MapGroup("/events")
                          .WithTags("Events").WithOpenApi()
                          .AddEndpointFilter(new IdempotencyKeyFilter());

        group.MapPost("", async (EventCreateDto dto, ILoggerFactory loggerFactory, IdempotencyKey idempotencyKey, IEventsService eventsService, CancellationToken ct) =>
        {
            var logger = loggerFactory.CreateLogger("Api.Events");

            logger.LogWarning(LogEvents.Info,
            "Accepting event {Type} at {OccurredAt}",
            dto.Type, dto.OccurredAt);
            
            var errors = new Dictionary<string, string[]>();
            if (string.IsNullOrWhiteSpace(dto.Type))
                errors[nameof(dto.Type)] = ["Type is required."];
            if (dto.OccurredAt == default)
                errors[nameof(dto.OccurredAt)] = ["OccurredAt must be a valid timestamp."];

            if (errors.Count > 0)
                return ProblemFactory.BadRequest400(nameof(EventsEndpoints), "Validation failed", errors);

            var result = await eventsService.CreateAsync(dto, idempotencyKey, ct);

            return result.IsReplay ? Results.Ok(result.Event) : Results.Accepted(value: result.Event);
        })
        .WithName("PostEvents")
        .ProducesValidationProblem(400)
        .Produces(StatusCodes.Status202Accepted);

        return routes;
    }
}
