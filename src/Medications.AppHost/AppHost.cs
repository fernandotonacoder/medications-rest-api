using Microsoft.Extensions.Configuration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var medicationsDb = !string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("medications"))
    ? builder.AddConnectionString("medications")
    : builder.AddSqlServer("sql")
        .WithImageTag("2022-CU26-ubuntu-22.04")
        .WithContainerName("medications-sql")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume()
        .AddDatabase("medications");

builder.AddProject<Medications_Api>("medications-api")
    .WithReference(medicationsDb)
    .WaitFor(medicationsDb)
    .WithUrlForEndpoint("http", url => new() { Url = "/scalar", DisplayText = "Scalar UI", DisplayOrder = 100 });

await builder.Build().RunAsync();
