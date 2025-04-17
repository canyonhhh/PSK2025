using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithDataVolume(isReadOnly: false);
var postgresdb = postgres.AddDatabase("postgresdb");

var apiService = builder.AddProject<Projects.PSK2025_ApiService>("apiservice")
    .WithReference(postgresdb)
    .WithHttpsEndpoint(name: "https1")
    .WithExternalHttpEndpoints();

builder.AddProject<PSK2025_MigrationService>("migrations")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

builder.AddNpmApp("webfrontend", "../PSK2025.Frontend")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("services__api__http__0", apiService.GetEndpoint("http"))
    .WithEnvironment("services__api__https__0", apiService.GetEndpoint("https"))
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();
builder.Build().Run();