using Medications.Api.Contracts;
using Medications.Api.Data;
using Medications.Api.Mapping;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medications.Api.Endpoints;

public static class MedicationEndpoints
{
    private const string GetMedicationsEndpoint = "GetMedications";
    private const string GetMedicationEndpoint = "GetMedication";
    private const string CreateMedicationEndpoint = "CreateMedication";
    private const string DeleteMedicationEndpoint = "DeleteMedication";


    public static IEndpointRouteBuilder MapMedicationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/medications")
            .WithTags("Medications");

        group.MapGet("/", GetMedications)
            .WithName(GetMedicationsEndpoint)
            .WithDescription("Retrieve all medications.");

        group.MapGet("/{id}", GetMedication)
            .WithName(GetMedicationEndpoint)
            .WithDescription("Retrieve a specific medication by its ID.");

        group.MapPost("/", CreateMedication)
            .WithName(CreateMedicationEndpoint)
            .WithDescription("Create a new medication");

        group.MapDelete("/{id}", DeleteMedication)
            .WithName(DeleteMedicationEndpoint)
            .WithDescription("Delete a medication");

        return endpoints;
    }

    #region Internal Methods

    internal static async Task<Ok<List<MedicationResponse>>> GetMedications(MedicationsDbContext dbContext)
    {
        var medications = await dbContext.Medications.ToListAsync();

        var medicationResponseList = medications.Select(medication => medication.ToResponse()).ToList();

        return TypedResults.Ok(medicationResponseList);
    }

    internal static async Task<Results<Ok<MedicationResponse>, NotFound>> GetMedication(
        int id, MedicationsDbContext dbContext, CancellationToken cancellationToken)
    {
        var medication = await dbContext.Medications.FindAsync(id, cancellationToken);

        if (medication is null) return TypedResults.NotFound();

        return TypedResults.Ok(medication.ToResponse());
    }

    internal static async Task<CreatedAtRoute<MedicationResponse>> CreateMedication(
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

    internal static async Task<Results<NoContent, NotFound>> DeleteMedication(
        int id, MedicationsDbContext dbContext, CancellationToken cancellationToken)
    {
        var medication = await dbContext.Medications.FindAsync(id, cancellationToken);

        if (medication is null) return TypedResults.NotFound();

        dbContext.Medications.Remove(medication);
        await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }

    #endregion

}
