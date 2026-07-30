using Microsoft.EntityFrameworkCore;

namespace MedicationsApi;

public class MedicationsDb : DbContext
{
    public MedicationsDb(DbContextOptions<MedicationsDb> options)
        : base(options) { }

    public DbSet<Medication> Medications => Set<Medication>();
}