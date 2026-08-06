namespace Medications.Api.Entities;

public class Medication
{
    public const int NameMaxLength = 200;

    public int Id { get; set; }

    public required string Name { get; set; }

    public int Quantity { get; set; }

    public required DateTimeOffset CreationDate { get; set; }
}
