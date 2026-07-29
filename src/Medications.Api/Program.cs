using MedicationsApi;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MedicationsDb>(opt => opt.UseInMemoryDatabase("Medications"));
builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// app.UseHttpsRedirection();

app.MapGet("/medications", async (MedicationsDb db) =>
{
    await db.Medications.ToListAsync();
})
.WithName("GetMedications");

app.Run();
