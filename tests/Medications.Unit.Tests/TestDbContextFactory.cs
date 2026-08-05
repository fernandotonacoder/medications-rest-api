using Medications.Api.Data;
using Medications.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Medications.Unit.Tests
{
    /// <summary>
    /// Shared fixture helpers.
    /// </summary>
    internal static class TestDbContextFactory
    {
        internal static readonly DateTimeOffset CreationDate = new(2026, 1, 15, 9, 30, 0, TimeSpan.Zero);

        internal static MedicationsDbContext CreateContext(params Medication[] seed)
        {
            var options = new DbContextOptionsBuilder<MedicationsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dbContext = new MedicationsDbContext(options);

            dbContext.Medications.AddRange(seed);
            dbContext.SaveChanges();

            return dbContext;
        }

        internal static Medication NewMedication(int id) => new()
        {
            Id = id,
            Name = $"Test Medication {id}",
            Quantity = 10 * id,
            CreationDate = CreationDate
        };
    }
}
