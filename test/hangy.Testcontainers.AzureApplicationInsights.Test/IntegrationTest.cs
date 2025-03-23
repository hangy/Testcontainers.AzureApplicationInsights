using System.Net.Http.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace hangy.Testcontainers.AzureApplicationInsights.Test;

public class IntegrationTest
{
    private static readonly HttpClient _client = new ();

    [Fact]
    public async Task Tracked_Event_Gets_Stored_And_Can_Be_Retrieved()
    {
        AppInsightsBuilder builder = new();
        string? imageName = Environment.GetEnvironmentVariable("APPINSIGHTS_IMAGE");
        if (!string.IsNullOrWhiteSpace(imageName))
        {
            builder = builder.WithImage(imageName);
        }
        
        AppInsightsContainer container = builder.Build();

        await container.StartAsync(TestContext.Current.CancellationToken);

        using (TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault())
        {            
            configuration.ConnectionString = container.GetConnectionString();
            TelemetryClient client = new (configuration);
            client.TrackTrace("Hello, World!");

            await client.FlushAsync(TestContext.Current.CancellationToken).ConfigureAwait(true);
        }

        var requests = _client.GetFromJsonAsAsyncEnumerable<TrackedRequest>($"{container.GetApiUrl()}trackedRequests", TestContext.Current.CancellationToken).ConfigureAwait(true);
        bool atLeastOne = false;
        await foreach(TrackedRequest? request in requests)
        {
            Assert.NotNull(request);
            Assert.Contains("\"message\":\"Hello, World!\"", request.Body, StringComparison.InvariantCulture);
            atLeastOne = true;
        }

        Assert.True(atLeastOne, "At least one request should have been tracked");    }

    public record TrackedRequest(Guid Id,
        DateTimeOffset Timestamp,
        IDictionary<string, IList<string?>> Headers,
        string Body);
}
