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
    }

    private static async Task<Ok<List<Medication>>> GetAllMedications(MedicationsDb db)
    {
        var medications = await db.Medications.ToListAsync();
        return TypedResults.Ok(medications);
    }
}