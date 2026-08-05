using Medications.Api.Contracts;
using Medications.Api.Data;
using Medications.Api.Endpoints;
using Medications.Api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medications.Unit.Tests
{
    public class GetByIdEndpointUnitTests
    {
        [Fact]
        public async Task GetMedicationById_WhenMedicationExists_ReturnsOK()
        {
            var medication1 = NewMedication(1);
            var medication2 = NewMedication(2);
            await using var dbContext = CreateContext(medication1, medication2);

            var result1 = await MedicationEndpoints.GetMedication(1, dbContext, CancellationToken.None);
            var result2 = await MedicationEndpoints.GetMedication(2, dbContext, CancellationToken.None);

            var response1 = Assert.IsType<Ok<MedicationResponse>>(result1.Result);
            Assert.Equal(StatusCodes.Status200OK, response1.StatusCode);
            Assert.NotNull(response1.Value);
            Assert.Equal(medication1.Id, response1.Value.Id);
            Assert.Equal(medication1.Name, response1.Value.Name);
            Assert.Equal(medication1.Quantity, response1.Value.Quantity);
            Assert.Equal(medication1.CreationDate, response1.Value.CreationDate);

            var response2 = Assert.IsType<Ok<MedicationResponse>>(result2.Result);
            Assert.Equal(StatusCodes.Status200OK, response2.StatusCode);
            Assert.NotNull(response2.Value);
            Assert.Equal(medication2.Id, response2.Value.Id);
            Assert.Equal(medication2.Name, response2.Value.Name);
            Assert.Equal(medication2.Quantity, response2.Value.Quantity);
            Assert.Equal(medication2.CreationDate, response2.Value.CreationDate);
        }

        [Fact]
        public async Task GetMedicationById_WhenMedicationDoesNotExist_ReturnsNotFound()
        {
            await using var dbContext = CreateContext(NewMedication(1), NewMedication(2));

            var result1 = await MedicationEndpoints.GetMedication(3, dbContext, CancellationToken.None);

            var response1 = Assert.IsType<NotFound>(result1.Result);
            Assert.Equal(StatusCodes.Status404NotFound, response1.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetMedicationById_WhenNotGreaterThanZero_ReturnsBadRequest(int id)
        {
            await using var dbContext = CreateContext(NewMedication(1), NewMedication(2));

            var result = await MedicationEndpoints.GetMedication(id, dbContext, CancellationToken.None);

            var problem = Assert.IsType<ProblemHttpResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
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
