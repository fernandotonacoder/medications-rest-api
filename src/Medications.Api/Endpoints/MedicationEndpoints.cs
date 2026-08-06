using System.ComponentModel.DataAnnotations;
using Medications.Api.Contracts;
using Medications.Api.Data;
using Medications.Api.Mapping;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medications.Api.Endpoints;

public static class MedicationEndpoints
{
    private const string RouteGroupPrefix = "/api/medications";

    private const string GetMedicationsEndpointName = "GetMedications";
    private const string GetMedicationEndpointName = "GetMedication";
    private const string CreateMedicationEndpointName = "CreateMedication";
    private const string DeleteMedicationEndpointName = "DeleteMedication";

    public static IEndpointRouteBuilder MapMedicationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(RouteGroupPrefix)
            .WithTags("Medications");

        group.MapGet("/", GetMedications)
            .WithName(GetMedicationsEndpointName)
            .WithDescription("Retrieve all medications.");

        group.MapGet("/{id}", GetMedication)
            .WithName(GetMedicationEndpointName)
            .WithDescription("Retrieve a specific medication by its ID.")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateMedication)
            .WithName(CreateMedicationEndpointName)
            .WithDescription("Create a new medication")
            .ProducesValidationProblem();

        group.MapDelete("/{id}", DeleteMedication)
            .WithName(DeleteMedicationEndpointName)
            .WithDescription("Delete a medication")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }

    internal static async Task<Ok<List<MedicationResponse>>> GetMedications(
        MedicationsDbContext dbContext, CancellationToken cancellationToken)
    {
        var medications = await dbContext.Medications
            .OrderBy(medication => medication.Id)
            .ToListAsync(cancellationToken);

        var medicationResponseList = medications.Select(medication => medication.ToResponse()).ToList();

        return TypedResults.Ok(medicationResponseList);
    }

    internal static async Task<Results<Ok<MedicationResponse>, NotFound>> GetMedication(
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than zero.")] int id,
        MedicationsDbContext dbContext, CancellationToken cancellationToken)
    {
        var medication = await dbContext.Medications.FindAsync([id], cancellationToken);

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

        return TypedResults.CreatedAtRoute(medicationResponse, GetMedicationEndpointName, new { id = medicationResponse.Id });
    }

    internal static async Task<Results<NoContent, NotFound>> DeleteMedication(
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than zero.")] int id,
        MedicationsDbContext dbContext, CancellationToken cancellationToken)
    {
        var medication = await dbContext.Medications.FindAsync([id], cancellationToken);

        if (medication is null) return TypedResults.NotFound();

        dbContext.Medications.Remove(medication);
        await dbContext.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }
}
