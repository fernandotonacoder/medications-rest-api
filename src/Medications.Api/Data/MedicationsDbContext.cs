using Medications.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Medications.Api.Data;

public class MedicationsDbContext(DbContextOptions<MedicationsDbContext> options) : DbContext(options)
{
    public DbSet<Medication> Medications => Set<Medication>();
}