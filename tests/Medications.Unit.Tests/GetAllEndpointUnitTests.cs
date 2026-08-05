using Medications.Api.Contracts;
using Medications.Api.Data;
using Medications.Api.Endpoints;
using Medications.Api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medications.Unit.Tests
{
    public class GetAllEndpointUnitTests
    {
        [Fact]
        public async Task GetAllMedications_WhenMedicationsExist_ReturnsOK()
        {
            var medication1 = NewMedication(1);
            var medication2 = NewMedication(2);
            await using var dbContext = CreateContext(medication1, medication2);

            var result = await MedicationEndpoints.GetMedications(dbContext, CancellationToken.None);

            var response = Assert.IsType<Ok<List<MedicationResponse>>>(result);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.NotNull(response.Value);

            Assert.NotNull(response.Value[0]);
            Assert.Equal(medication1.Id, response.Value[0].Id);
            Assert.Equal(medication1.Name, response.Value[0].Name);
            Assert.Equal(medication1.Quantity, response.Value[0].Quantity);
            Assert.Equal(medication1.CreationDate, response.Value[0].CreationDate);

            Assert.NotNull(response.Value[1]);
            Assert.Equal(medication2.Id, response.Value[1].Id);
            Assert.Equal(medication2.Name, response.Value[1].Name);
            Assert.Equal(medication2.Quantity, response.Value[1].Quantity);
            Assert.Equal(medication2.CreationDate, response.Value[1].CreationDate);
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
