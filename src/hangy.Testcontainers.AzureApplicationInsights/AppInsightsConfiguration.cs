using Docker.DotNet.Models;
using DotNet.Testcontainers.Configurations;

namespace hangy.Testcontainers.AzureApplicationInsights;

/// <inheritdoc cref="ContainerConfiguration" />
public class AppInsightsConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppInsightsConfiguration" /> class.
    /// </summary>
    public AppInsightsConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppInsightsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AppInsightsConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppInsightsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AppInsightsConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppInsightsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AppInsightsConfiguration(AppInsightsConfiguration resourceConfiguration)
        : this(new AppInsightsConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppInsightsConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public AppInsightsConfiguration(AppInsightsConfiguration oldValue, AppInsightsConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}
