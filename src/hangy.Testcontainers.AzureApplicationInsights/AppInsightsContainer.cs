using DotNet.Testcontainers.Containers;

namespace hangy.Testcontainers.AzureApplicationInsights;

/// <inheritdoc cref="DockerContainer" />
public class AppInsightsContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppInsightsContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public AppInsightsContainer(AppInsightsConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the AppInsights connection string.
    /// </summary>
    /// <returns>The AppInsights connection string.</returns>
    public string GetConnectionString()
        => $"InstrumentationKey={Guid.NewGuid()};IngestionEndpoint={GetApiUrl()};ApplicationId=${Guid.NewGuid()}";

    /// <summary>
    /// Gets the AppInsights API URL
    /// </summary>
    /// <returns>The AppInsights API URL.</returns>
    public Uri GetApiUrl()
        => new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AppInsightsBuilder.AppInsightsPort)).Uri;
}
