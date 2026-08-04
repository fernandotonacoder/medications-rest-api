using Medications.Api.Data;
using Medications.Api.Endpoints;
using Medications.Api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medications.Unit.Tests
{
    public class EndpointUnitTests
    {
        [Fact]
        public async Task DeleteMedicationById_WhenMedicationExists_ReturnsNoContent()
        {
            await using var dbContext = CreateContext(NewMedication(1), NewMedication(2));

            var result = await MedicationEndpoints.DeleteMedication(1, dbContext, CancellationToken.None);

            Assert.IsType<NoContent>(result.Result);
            Assert.Null(dbContext.Medications.Find(1));
            Assert.NotNull(dbContext.Medications.Find(2));
        }

        [Fact]
        public async Task DeleteMedicationById_WhenMedicationDoesNotExist_ReturnsNotFound()
        {
            await using var dbContext = CreateContext(NewMedication(1), NewMedication(2));

            var result = await MedicationEndpoints.DeleteMedication(3, dbContext, CancellationToken.None);

            Assert.IsType<NotFound>(result.Result);
            Assert.NotNull(dbContext.Medications.Find(1));
            Assert.NotNull(dbContext.Medications.Find(2));
        }

        [Fact]
        public async Task DeleteMedicationById_WhenIdIsNegative_ReturnsBadRequest()
        {
            await using var dbContext = CreateContext(NewMedication(1), NewMedication(2));

            var result = await MedicationEndpoints.DeleteMedication(-1, dbContext, CancellationToken.None);

            var problem = Assert.IsType<ProblemHttpResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
            Assert.NotNull(dbContext.Medications.Find(1));
            Assert.NotNull(dbContext.Medications.Find(2));
        }

        private static MedicationsDbContext CreateContext(params Medication[] seed)
        {
            var options = new DbContextOptionsBuilder<MedicationsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dbContext = new MedicationsDbContext(options);

            dbContext.Medications.AddRange(seed);
            dbContext.SaveChanges();

            return dbContext;
        }

        private static Medication NewMedication(int id) => new()
        {
            Id = id,
            Name = $"Test Medication {id}",
            Quantity = 10 * id,
            CreationDate = DateTimeOffset.UtcNow
        };
    }
}
