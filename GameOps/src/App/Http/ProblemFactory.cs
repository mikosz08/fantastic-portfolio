using Microsoft.AspNetCore.Mvc;

namespace App.Http;

public static class ProblemFactory
{
    public static IResult BadRequest(string title, IDictionary<string, string[]>? errors = null)
    {
        var pd = new ValidationProblemDetails(errors ?? new Dictionary<string, string[]>())
        {
            Title = title,
            Status = StatusCodes.Status400BadRequest
        };
        return Results.Problem(pd);
    }
}