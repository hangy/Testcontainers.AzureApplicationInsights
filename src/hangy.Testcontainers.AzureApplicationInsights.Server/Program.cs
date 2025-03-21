WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/v2.1/track", async (HttpRequest request, CancellationToken cancellationToken) =>
{
    using StreamReader reader = new(request.Body);
    string body = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
    app.Logger.LogDebug("Received telemetry: {Telemetry}", body);
    return Results.NoContent();
})
.WithName("TrackApplicationInsights");

await app.RunAsync().ConfigureAwait(false);
