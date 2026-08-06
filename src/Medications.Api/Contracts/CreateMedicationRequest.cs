using System.ComponentModel.DataAnnotations;
using Medications.Api.Entities;

namespace Medications.Api.Contracts;

public class CreateMedicationRequest
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(Medication.NameMaxLength)]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}
