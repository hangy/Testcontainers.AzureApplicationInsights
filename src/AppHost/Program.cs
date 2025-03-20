using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<hangy_Testcontainers_AzureApplicationInsights_Server>("api")
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync().ConfigureAwait(false);
