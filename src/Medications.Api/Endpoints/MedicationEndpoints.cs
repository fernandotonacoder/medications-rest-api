using Medications.Api.Data;
using Medications.Api.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medications.Api.Endpoints;

public static class MedicationEndpoints
{
    public static void MapMedicationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/medications")
            .WithTags("Medications");

        group.MapGet("/", GetAllMedications)
        .WithName("GetMedications")
        .WithDescription("Retrieve all medications.");
        
        group.MapGet("/{id}", GetMedication)
            .WithName("GetMedication")
            .WithDescription("Retrieve a specific medication by its ID.");
    }

    private static async Task<Ok<List<Medication>>> GetAllMedications(MedicationsDbContext dbContext)
    {
        var medications = await dbContext.Medications.ToListAsync();
        return TypedResults.Ok(medications);
    }

    private static async Task<Results<Ok<Medication>, NotFound>> GetMedication(int id, MedicationsDbContext dbContext)
    {
        var medication = await dbContext.Medications.FindAsync(id);
        
        if (medication is null) return TypedResults.NotFound();

        return TypedResults.Ok(medication);
    }
}