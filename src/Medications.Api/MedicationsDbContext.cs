using Microsoft.EntityFrameworkCore;

namespace Medications.Api;

public class MedicationsDbContext(DbContextOptions<MedicationsDbContext> options) : DbContext(options)
{
    public DbSet<Medication> Medications => Set<Medication>();
}