namespace MedicationsApi;

public class MedicationModel
{
    /// <summary>
    /// Gets or sets the ID of the medication.
    /// </summary>
    public int MedicationId { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the medication.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the medication.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the medication.
    /// </summary>
    public DateTime CreationDate { get; set; }
}