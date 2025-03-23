using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

namespace hangy.Testcontainers.AzureApplicationInsights;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
/// <remarks>
/// Builds a container running the an Azure Application Insights emulator:
/// https://github.com/hangy/Testcontainers.AzureApplicationInsights/.
/// </remarks>
public class AppInsightsBuilder : ContainerBuilder<AppInsightsBuilder, AppInsightsContainer, AppInsightsConfiguration>
{
    public const ushort AppInsightsPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppInsightsBuilder" /> class.
    /// </summary>
    public AppInsightsBuilder()
        : this(new AppInsightsConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppInsightsBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private AppInsightsBuilder(AppInsightsConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    public string AppInsightsImage  { get; } = $"ghcr.io/hangy/azure-appinsights-emulator:{ThisAssembly.Info.Version}";

    /// <inheritdoc />
    protected override AppInsightsConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override AppInsightsContainer Build()
    {
        Validate();
        return new AppInsightsContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override AppInsightsBuilder Init()
    {
        return base.Init()
            .WithImage(AppInsightsImage)
            .WithPortBinding(AppInsightsPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request
                .WithMethod(HttpMethod.Get)
                .ForPort(AppInsightsPort)
                .ForPath("/health")));
    }

    /// <inheritdoc />
    protected override AppInsightsBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AppInsightsConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AppInsightsBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AppInsightsConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AppInsightsBuilder Merge(AppInsightsConfiguration oldValue, AppInsightsConfiguration newValue)
    {
        return new AppInsightsBuilder(new AppInsightsConfiguration(oldValue, newValue));
    }
}
