using Projects;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Medications_Api>("medications-api");

var scalar = builder.AddScalarApiReference("scalar", options =>
{
    options
        .WithTheme(ScalarTheme.Default)
        .PreferHttpsEndpoint()
        .AllowSelfSignedCertificates();
});

scalar
    .WithApiReference(apiService);

await builder.Build().RunAsync();
