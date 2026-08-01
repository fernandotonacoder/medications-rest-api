using System.ComponentModel.DataAnnotations;

namespace Medications.Api.Contracts;

public class CreateMedicationRequest
{

    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}
