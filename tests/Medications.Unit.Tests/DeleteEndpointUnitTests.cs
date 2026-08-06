using Medications.Api.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using static Medications.Unit.Tests.TestDbContextFactory;

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
    }
}
