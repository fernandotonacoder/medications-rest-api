using Medications.Api.Contracts;
using Medications.Api.Data;
using Medications.Api.Mapping;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medications.Api.Endpoints;

public static class MedicationEndpoints
{
    public static IEndpointRouteBuilder MapMedicationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/medications")
            .WithTags("Medications");

        group.MapGet("/", GetAllMedications)
            .WithName("GetMedications")
            .WithDescription("Retrieve all medications.");

        group.MapGet("/{id}", GetMedication)
            .WithName("GetMedication")
            .WithDescription("Retrieve a specific medication by its ID.");

        group.MapPost("/", CreateMedication)
            .WithName("CreateMedication")
            .WithDescription("Create a new medication");

        return endpoints;
    }

    private static async Task<Ok<List<MedicationResponse>>> GetAllMedications(MedicationsDbContext dbContext)
    {
        var medications = await dbContext.Medications.ToListAsync();

        var medicationResponseList = medications.Select(medication => medication.ToResponse()).ToList();

        return TypedResults.Ok(medicationResponseList);
    }

    private static async Task<Results<Ok<MedicationResponse>, NotFound>> GetMedication(
        int id, MedicationsDbContext dbContext)
    {
        var medication = await dbContext.Medications.FindAsync(id);

        if (medication is null) return TypedResults.NotFound();

        return TypedResults.Ok(medication.ToResponse());
    }

    private static async Task<CreatedAtRoute<MedicationResponse>> CreateMedication(
        CreateMedicationRequest request,
        MedicationsDbContext dbContext,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var medication = request.ToMedication(timeProvider);

        dbContext.Add(medication);

        await dbContext.SaveChangesAsync(cancellationToken);

        var medicationResponse = medication.ToResponse();

        return TypedResults.CreatedAtRoute(medicationResponse, "GetMedication", new { id = medicationResponse.Id });
    }
}
