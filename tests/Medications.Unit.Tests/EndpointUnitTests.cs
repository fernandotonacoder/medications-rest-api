using Medications.Api.Data;
using Medications.Api.Endpoints;
using Medications.Api.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medications.Unit.Tests
{
    public class EndpointUnitTests
    {
        [Fact]
        public async Task DeleteMedicationById_WhenMedicationExists_ReturnsNoContent()
        {
            var medication1 = new Medication
            {
                Id = 1,
                Name = "Test Medication",
                Quantity = 10,
                CreationDate = DateTimeOffset.UtcNow
            };

            var medication2 = new Medication
            {
                Id = 2,
                Name = "Test Medication 2",
                Quantity = 20,
                CreationDate = DateTimeOffset.UtcNow
            };

            var dbContextOptions = new DbContextOptionsBuilder<MedicationsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var dbContext = new MedicationsDbContext(dbContextOptions);


            dbContext.Medications.Add(medication1);
            dbContext.Medications.Add(medication2);
            dbContext.SaveChanges();

            var result = await MedicationEndpoints.DeleteMedication(1, dbContext, CancellationToken.None);

            var value1 = dbContext.Medications.Find(1);
            var value2 = dbContext.Medications.Find(2);


            Assert.IsType<NoContent>(result.Result);
            Assert.Null(value1);
            Assert.NotNull(value2);
        }
    }
}
