using Medications.Api.Contracts;
using Medications.Api.Entities;

namespace Medications.Api.Mapping;

public static class MedicationMapping
{
    public static Medication ToMedication(this CreateMedicationRequest request, TimeProvider timeProvider)
    {
        return new Medication
        {
            Name = request.Name.Trim(),
            Quantity = request.Quantity,
            CreationDate = timeProvider.GetUtcNow()
        };
    }

    public static MedicationResponse ToResponse(this Medication medication)
    {
        return new MedicationResponse(
            medication.Id,
            medication.Name,
            medication.Quantity,
            medication.CreationDate);
    }
}
