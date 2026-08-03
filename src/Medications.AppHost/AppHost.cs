using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Medications_Api>("medications-api")
    .WithUrlForEndpoint("https", url => new() { Url = "/scalar", DisplayText = "Scalar UI", DisplayOrder = 100 });

await builder.Build().RunAsync();
