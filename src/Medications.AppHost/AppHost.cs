using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.MedicationsApi>("medicationsapi");

var scalar = builder.AddScalarApiReference("scalar");

scalar
    .WithApiReference(apiService);

await builder.Build().RunAsync();