using Microsoft.Extensions.Primitives;

namespace App.Http;

public static class IdempotencyKey
{
    public const string HeaderName = "Idempotency-Key";

    public static bool TryGet(HttpRequest req, out string key)
    {
        key = string.Empty;

        if (!req.Headers.TryGetValue(HeaderName, out StringValues values))
            return false;

        for (int i = 0; i < values.Count; i++)
        {
            var v = values[i];                  
            if (!string.IsNullOrWhiteSpace(v))  
            {
                key = v.Trim();                 
                return true;
            }
        }

        return false;
    }
}
