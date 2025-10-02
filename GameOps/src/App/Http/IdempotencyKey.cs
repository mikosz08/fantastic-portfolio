using System.Reflection;

namespace App.Http;

public readonly record struct IdempotencyKey(string Value)
{
    public static ValueTask<IdempotencyKey> BindAsync(HttpContext httpContext, ParameterInfo parameterInfo)
    {
        if (httpContext.Items.TryGetValue(IdempotencyKeyFilter.HttpItemsKey, out var itemsKey)
            && itemsKey is string key
            && !string.IsNullOrWhiteSpace(key))
        {
            return ValueTask.FromResult(new IdempotencyKey(key));
        }

        if (httpContext.Request.Headers.TryGetValue(IdempotencyKeyFilter.HeaderName, out var headerValues) && headerValues.Count > 0)
        {
            var headerKey = headerValues.FirstOrDefault(string.Empty)?.Trim();
            if (!string.IsNullOrWhiteSpace(headerKey))
                return ValueTask.FromResult(new IdempotencyKey(headerKey));
        }

        return ValueTask.FromResult(new IdempotencyKey(string.Empty));
    }

    public override string ToString() => Value;
    public static implicit operator string(IdempotencyKey key) => key.Value;
}
