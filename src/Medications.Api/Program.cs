using Medications.Api.Data;
using Medications.Api.Endpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MedicationsDbContext>(opt => opt.UseInMemoryDatabase("Medications"));
builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        if (context.Exception is BadHttpRequestException badRequest)
        {
            context.ProblemDetails.Detail ??= badRequest.Message;
        }
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    StatusCodeSelector = ex => ex is BadHttpRequestException badRequest
        ? badRequest.StatusCode
        : StatusCodes.Status500InternalServerError,
    SuppressDiagnosticsCallback = context => context.Exception is BadHttpRequestException
});
app.UseStatusCodePages();

app.UseHttpsRedirection();

app.MapMedicationEndpoints();

await app.RunAsync();
