using Microsoft.AspNetCore.Mvc;

public static class ProblemFactory
{
    public static IResult BadRequest(string source, string title, IDictionary<string, string[]>? errors = null)
    {
        var pd = new ValidationProblemDetails(errors ?? new Dictionary<string, string[]>())
        {
            Title = string.IsNullOrWhiteSpace(source) ? title : $"[{source}] {title}",
            Status = StatusCodes.Status400BadRequest
        };
        return Results.Problem(pd);
    }
}
