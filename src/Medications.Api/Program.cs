using MedicationsApi;
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

app.MapGet("/medications", async (MedicationsDb db) =>
{
    await db.Medications.ToListAsync();
})
.WithName("GetMedications");

await app.RunAsync();
