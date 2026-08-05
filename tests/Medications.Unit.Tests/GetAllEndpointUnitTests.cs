using Medications.Api.Contracts;
using Medications.Api.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using static Medications.Unit.Tests.TestDbContextFactory;

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
            Assert.NotNull(response.Value);
            Assert.Equal(2, response.Value.Count);

            // Index order is meaningful because the endpoint orders by id.
            Assert.Equal(medication1.Id, response.Value[0].Id);
            Assert.Equal(medication1.Name, response.Value[0].Name);
            Assert.Equal(medication1.Quantity, response.Value[0].Quantity);
            Assert.Equal(medication1.CreationDate, response.Value[0].CreationDate);

            Assert.Equal(medication2.Id, response.Value[1].Id);
            Assert.Equal(medication2.Name, response.Value[1].Name);
            Assert.Equal(medication2.Quantity, response.Value[1].Quantity);
            Assert.Equal(medication2.CreationDate, response.Value[1].CreationDate);
        }

        [Fact]
        public async Task GetAllMedications_WhenNoMedicationsExist_ReturnsEmptyList()
        {
            await using var dbContext = CreateContext();

            var result = await MedicationEndpoints.GetMedications(dbContext, CancellationToken.None);

            var response = Assert.IsType<Ok<List<MedicationResponse>>>(result);
            Assert.NotNull(response.Value);
            Assert.Empty(response.Value);
        }
    }
}
