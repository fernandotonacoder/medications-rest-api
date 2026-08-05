using Medications.Api.Contracts;
using Medications.Api.Data;
using Medications.Api.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

namespace Medications.Unit.Tests
{
    public class PostEndpointUnitTests
    {
        [Fact]
        public async Task PostMedication_Returns201CreatedAtRoute()
        {
            await using var dbContext = CreateContext();
            var medicationRequest = GenerateCreateMedicationRequest("Paracetamol", 10);
            var clock = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 9, 30, 0, TimeSpan.Zero));

            var result = await MedicationEndpoints.CreateMedication(
                medicationRequest, dbContext, clock, CancellationToken.None);

            var response = Assert.IsType<CreatedAtRoute<MedicationResponse>>(result);
            Assert.NotNull(response.Value);
            Assert.Equal(1, response.Value.Id);
            Assert.Equal("GetMedication", response.RouteName);
            Assert.Equal(1, response.RouteValues["id"]);
            Assert.Equal(medicationRequest.Name, response.Value.Name);
            Assert.Equal(medicationRequest.Quantity, response.Value.Quantity);

            var persistedMedication = dbContext.Medications.Find(1);
            Assert.NotNull(persistedMedication);
            Assert.Equal(medicationRequest.Name, persistedMedication.Name);
            Assert.Equal(medicationRequest.Quantity, persistedMedication.Quantity);
            Assert.Equal(clock.GetUtcNow(), persistedMedication.CreationDate);
        }

        private static MedicationsDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<MedicationsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new MedicationsDbContext(options);
        }

        private static CreateMedicationRequest GenerateCreateMedicationRequest(string name, int quantity)
        {
            return new() { Name = name, Quantity = quantity };
        }
    }
}
