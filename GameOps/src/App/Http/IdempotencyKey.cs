using System.Reflection;

namespace App.Http;

public readonly record struct IdempotencyKey(string Value)
{
    public static ValueTask<IdempotencyKey> BindAsync(HttpContext httpContext, ParameterInfo parameterInfo)
    {
        if (httpContext.Items.TryGetValue(IdempotencyKeyFilter.HttpItemsKey, out var itemValue)
            && itemValue is string fromItems
            && !string.IsNullOrWhiteSpace(fromItems))
        {
            return ValueTask.FromResult(new IdempotencyKey(fromItems));
        }

        if (httpContext.Request.Headers.TryGetValue(IdempotencyKeyFilter.HeaderName, out var headerValues) && headerValues.Count > 0)
        {
            var rawHeaderValue = headerValues[0];
            if (!string.IsNullOrWhiteSpace(rawHeaderValue))
                return ValueTask.FromResult(new IdempotencyKey(rawHeaderValue.Trim()));
        }

        return ValueTask.FromResult(new IdempotencyKey(string.Empty));
    }

    public override string ToString() => Value;
    public static implicit operator string(IdempotencyKey key) => key.Value;
}
