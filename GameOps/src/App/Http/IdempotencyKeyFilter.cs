using Microsoft.AspNetCore.Mvc;

namespace App.Http;

public sealed class IdempotencyKeyFilter : IEndpointFilter
{
    public const string HeaderName = "Idempotency-Key";
    public const string HttpItemsKey = "IdempotencyKey";
    private const string Source = nameof(IdempotencyKeyFilter);

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var req = ctx.HttpContext.Request;

        if (!req.Headers.TryGetValue(HeaderName, out var values) || values.Count == 0)
            return ProblemFactory.BadRequest(Source, "Missing Idempotency-Key header");

        var raw = values[0]; 
        if (string.IsNullOrWhiteSpace(raw))
            return ProblemFactory.BadRequest(Source, "Idempotency-Key cannot be empty");

        var key = raw.Trim(); 
        ctx.HttpContext.Items[HttpItemsKey] = key;

        return await next(ctx);
    }
}
