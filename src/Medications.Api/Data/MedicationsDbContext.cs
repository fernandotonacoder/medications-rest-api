using Medications.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Medications.Api.Data;

public class MedicationsDbContext(
    DbContextOptions<MedicationsDbContext> options) : DbContext(options)
{
    public DbSet<Medication> Medications => Set<Medication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Medication>()
            .Property(medication => medication.Name)
            .HasMaxLength(Medication.NameMaxLength);
    }
}
