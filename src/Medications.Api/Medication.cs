namespace Medications.Api;

public class Medication
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int Quantity { get; set; }

    public required DateTimeOffset CreationDate { get; set; }
}
