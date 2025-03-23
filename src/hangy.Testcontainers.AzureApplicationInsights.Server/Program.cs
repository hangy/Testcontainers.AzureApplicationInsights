using System.Text;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Expressions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpLogging();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<List<TrackedRequest>>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpLogging();
app.UseHttpsRedirection();

async Task<IResult> TrackRequestAsync(HttpRequest request, TimeProvider timeProvider, List<TrackedRequest> trackedRequests, CancellationToken cancellationToken)
{
    if (!request.HasJsonContentType() && request.ContentType != "application/x-json-stream")
    {
        return Results.BadRequest("Expected JSON or JSON stream content type");
    }

    DateTimeOffset now = timeProvider.GetLocalNow();
    Guid batchId = Guid.CreateVersion7();

    request.EnableBuffering();
    using MemoryStream mem = new();
    await request.Body.CopyToAsync(mem, cancellationToken).ConfigureAwait(false);
    request.Body.Seek(0, SeekOrigin.Begin);

    byte[] arr =  mem.ToArray();
    app.Logger.LogInformation("B64: {Base64Body}", Convert.ToBase64String(arr));    

    using StreamReader reader = new(request.Body, Encoding.UTF8);
    List<string> lines = [];

    string? line;
    while ((line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) is not null)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            lines.Add(line);
        }
    }

    foreach (string jsonLine in lines)
    {
        Guid id = Guid.CreateVersion7();
        trackedRequests.Add(new TrackedRequest(batchId,
            id,
            now,
            request.Headers.ToDictionary(kvp => kvp.Key, kvp => (IList<string>)[.. kvp.Value]),
            jsonLine));

        app.Logger.LogInformation("Tracked request with id {Id} batch {BatchId}", id, batchId);
    }

    return Results.NoContent();
}

app.MapPost("/v2/track", TrackRequestAsync).WithName("TrackApplicationInsights2");
app.MapPost("/v2.1/track", TrackRequestAsync).WithName("TrackApplicationInsights21");

app.MapGet("/trackedRequests", (List<TrackedRequest> trackedReqeusts) =>
{
    return Results.Ok(trackedReqeusts);
});

app.MapDefaultEndpoints();

await app.RunAsync().ConfigureAwait(false);

public record TrackedRequest(Guid batchId,
    Guid Id,
    DateTimeOffset Timestamp,
    IDictionary<string, IList<string>> Headers,
    string Body);