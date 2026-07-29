var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.MedicationsApi>("medicationsapi");

await builder.Build().RunAsync();