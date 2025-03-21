using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ProjectResource> api = builder.AddProject<hangy_Testcontainers_AzureApplicationInsights_Server>("api")
    .WithExternalHttpEndpoints();

builder.AddProject<hangy_Testcontainers_AzureApplicationInsights_TestClient>("testclient")
    .WithReference(api)
    .WithExternalHttpEndpoints()
    .WithEnvironment("APPLICATIONINSIGHTS_CONNECTION_STRING", 
        $"InstrumentationKey=00000000-0000-0000-0000-000000000000;IngestionEndpoint={api.GetEndpoint("http")}");

await builder.Build().RunAsync().ConfigureAwait(false);
