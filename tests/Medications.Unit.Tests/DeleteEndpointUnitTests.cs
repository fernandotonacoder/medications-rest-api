using Medications.Api.Data;
using Medications.Api.Endpoints;
using Medications.Api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medications.Unit.Tests
{
    public class DeleteEndpointUnitTests
    {
        [Fact]
        public async Task DeleteMedication_WhenMedicationExists_ReturnsNoContent()
        {
            await using var dbContext = CreateContext(NewMedication(1), NewMedication(2));

            var result = await MedicationEndpoints.DeleteMedication(1, dbContext, CancellationToken.None);

            Assert.IsType<NoContent>(result.Result);
            Assert.Null(dbContext.Medications.Find(1));
            Assert.NotNull(dbContext.Medications.Find(2));
        }

        [Fact]
        public async Task DeleteMedication_WhenMedicationDoesNotExist_ReturnsNotFound()
        {
            await using var dbContext = CreateContext(NewMedication(1), NewMedication(2));

            var result = await MedicationEndpoints.DeleteMedication(3, dbContext, CancellationToken.None);

            Assert.IsType<NotFound>(result.Result);
            Assert.NotNull(dbContext.Medications.Find(1));
            Assert.NotNull(dbContext.Medications.Find(2));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task DeleteMedication_WhenIdNotGreaterThanZero_ReturnsBadRequest(int id)
        {
            await using var dbContext = CreateContext(NewMedication(1), NewMedication(2));

            var result = await MedicationEndpoints.DeleteMedication(id, dbContext, CancellationToken.None);

            var problem = Assert.IsType<ProblemHttpResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
            Assert.NotNull(dbContext.Medications.Find(1));
            Assert.NotNull(dbContext.Medications.Find(2));
            Assert.Equal("Id must be greater than zero.", problem.ProblemDetails.Detail);
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
            CreationDate = new DateTimeOffset(2026, 1, 15, 9, 30, 0, TimeSpan.Zero)
        };

    }
}
