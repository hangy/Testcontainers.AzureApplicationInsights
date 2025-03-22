using Microsoft.Extensions.Primitives;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<List<TrackedRequest>>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/v2.1/track", async (HttpRequest request, TimeProvider timeProvider, List<TrackedRequest> trackedReqeusts, CancellationToken cancellationToken) =>
{
    using StreamReader reader = new(request.Body);
    string body =  await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);;
    Guid id = Guid.CreateVersion7();
    trackedReqeusts.Add(new TrackedRequest(
        id,
        timeProvider.GetLocalNow(),
        request.Headers,
        body));

    app.Logger.LogInformation("Tracked request with id {Id}", id);

    return Results.NoContent();
})
.WithName("TrackApplicationInsights");

app.MapGet("/trackedRequests", (List<TrackedRequest> trackedReqeusts) =>
{
    return Results.Ok(trackedReqeusts);
});

await app.RunAsync().ConfigureAwait(false);

public record TrackedRequest(Guid Id,
    DateTimeOffset Timestamp,
    IDictionary<string, StringValues> Headers,
    string body);