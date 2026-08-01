using Medications.Api.Data;
using Medications.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MedicationsDbContext>(opt => opt.UseInMemoryDatabase("Medications"));
builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapMedicationEndpoints();

await app.RunAsync();
