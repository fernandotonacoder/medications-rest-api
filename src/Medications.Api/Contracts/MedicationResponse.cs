namespace Medications.Api.Contracts;

public sealed record MedicationResponse(int Id, string Name, int Quantity, DateTimeOffset CreationDate);
