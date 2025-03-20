var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/v2/track", static (HttpRequest request) =>
{
    return Results.Stream(request.Body, contentType: request.ContentType);
})
.WithName("TrackApplicationInsights");

await app.RunAsync().ConfigureAwait(false);
