using Microsoft.EntityFrameworkCore;

namespace Medications.Api;

public class MedicationsDb : DbContext
{
    public MedicationsDb(DbContextOptions<MedicationsDb> options)
        : base(options) { }

    public DbSet<Medication> Medications => Set<Medication>();
}