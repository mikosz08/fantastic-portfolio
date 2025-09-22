using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace GameOps.Tests.Unit;

public class EventDtoValidationTests : IClassFixture<TestingWebAppFactory>
{
    private readonly HttpClient _client;
    public EventDtoValidationTests(TestingWebAppFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task PostEvents_WithoutIdempotencyKey_Returns400ProblemDetails()
    {
        var body = new { type = "new_level_reached", occurredAt = DateTimeOffset.Parse("2025-09-15T12:34:56Z") };
        var req = PostJson("/events", body);

        var res = await _client.SendAsync(req); // no Idempotency-Key header
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var problem = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(400, problem!.Status);
        Assert.Equal("Missing Idempotency-Key header", problem.Title);
    }

    [Fact]
    public async Task PostEvents_MissingType_Returns400WithErrors()
    {
        var body = new { type = "   ", occurredAt = DateTimeOffset.Parse("2025-09-15T12:34:56Z") }; // empty type
        var req = PostJson("/events", body);
        req.Headers.Add("Idempotency-Key", "test-123");

        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var problem = await res.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(400, problem!.Status);
        Assert.Equal("Validation failed", problem.Title);

        Assert.True(problem.Errors.ContainsKey("Type"));
        Assert.Contains("Type is required.", problem.Errors["Type"]);
    }

    [Fact]
    public async Task PostEvents_DefaultOccurredAt_Returns400WithErrors()
    {
        var body = new { type = "item_collected", occurredAt = default(DateTimeOffset) }; // default occurredAt
        var req = PostJson("/events", body);
        req.Headers.Add("Idempotency-Key", "test-123");

        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var problem = await res.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(400, problem!.Status);
        Assert.Equal("Validation failed", problem.Title);

        Assert.True(problem.Errors.ContainsKey("OccurredAt"));
        Assert.Contains("OccurredAt must be a valid timestamp.", problem.Errors["OccurredAt"]);
    }

    [Fact]
    public async Task PostEvents_ValidRequest_Returns202AndEchoesRequest()
    {
        var body = new
            {
                type = "item_collected",
                occurredAt = DateTimeOffset.Parse("2025-09-15T12:34:56Z"),
                playerId = "p-001",
                sessionId = "s-abc",
                payload = new { itemId = 42, rarity = "epic" }
            };

        var req = PostJson("/events", body);
        req.Headers.Add("Idempotency-Key", "k-123");

        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.Accepted, res.StatusCode);

        var json = await res.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(json);

        // idempotencyKey
        Assert.Equal("k-123", (string?)json!["idempotencyKey"]);

        // request echo
        var reqEcho = json["request"]!.AsObject();
        Assert.Equal("item_collected", (string?)reqEcho["type"]);
        Assert.Equal("p-001", (string?)reqEcho["playerId"]);
        Assert.Equal("s-abc", (string?)reqEcho["sessionId"]);

        // occurredAt
        var occurredAtStr = (string?)reqEcho["occurredAt"];
        Assert.NotNull(occurredAtStr);
        Assert.Equal(DateTimeOffset.Parse("2025-09-15T12:34:56Z"),
                     DateTimeOffset.Parse(occurredAtStr!));

        // payload
        var payload = reqEcho["payload"]!.AsObject();
        Assert.Equal(42, (int?)payload["itemId"]);
        Assert.Equal("epic", (string?)payload["rarity"]);
    }

    private static HttpRequestMessage PostJson(string url, object body)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(body, options: new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        };
        return req;
    }
}
