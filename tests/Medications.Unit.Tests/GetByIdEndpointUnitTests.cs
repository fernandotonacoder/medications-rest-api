using Medications.Api.Contracts;
using Medications.Api.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using static Medications.Unit.Tests.TestDbContextFactory;

namespace Medications.Unit.Tests
{
    public class GetByIdEndpointUnitTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetMedicationById_WhenMedicationExists_ReturnsOK(int id)
        {
            var expected = NewMedication(id);
            await using var dbContext = CreateContext(NewMedication(1), NewMedication(2));

            var result = await MedicationEndpoints.GetMedication(id, dbContext, CancellationToken.None);

            var response = Assert.IsType<Ok<MedicationResponse>>(result.Result);
            Assert.NotNull(response.Value);
            Assert.Equal(expected.Id, response.Value.Id);
            Assert.Equal(expected.Name, response.Value.Name);
            Assert.Equal(expected.Quantity, response.Value.Quantity);
            Assert.Equal(expected.CreationDate, response.Value.CreationDate);
        }

        [Fact]
        public async Task GetMedicationById_WhenMedicationDoesNotExist_ReturnsNotFound()
        {
            await using var dbContext = CreateContext(NewMedication(1), NewMedication(2));

            var result = await MedicationEndpoints.GetMedication(3, dbContext, CancellationToken.None);

            Assert.IsType<NotFound>(result.Result);
        }
    }
}
