using MedicationsApi;
using MedicationsApi.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MedicationsDb>(opt => opt.UseInMemoryDatabase("Medications"));
builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapMedicationEndpoints();

await app.RunAsync();
