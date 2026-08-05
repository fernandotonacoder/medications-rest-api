using Medications.Api.Contracts;
using Medications.Api.Entities;
using Medications.Api.Mapping;
using Microsoft.Extensions.Time.Testing;

namespace Medications.Unit.Tests
{
    public class MedicationMappingUnitTests
    {
        [Fact]
        public void ToMedication_MapsCorrectly()
        {
            var clock = new FakeTimeProvider(new DateTimeOffset(2026, 1, 15, 9, 30, 0, TimeSpan.Zero));
            var request = new CreateMedicationRequest { Name = "  Aspirin  ", Quantity = 5 };

            var medication = request.ToMedication(clock);

            Assert.Equal(clock.GetUtcNow(), medication.CreationDate);
            Assert.Equal(request.Name.Trim(), medication.Name);
            Assert.Equal(5, medication.Quantity);
        }

        [Fact]
        public void ToResponse_MapsCorrectly()
        {
            var medication = new Medication
            {
                Name = "Aspirin",
                Quantity = 5,
                CreationDate = new DateTimeOffset(2026, 1, 15, 9, 30, 0, TimeSpan.Zero)
            };

            var response = medication.ToResponse();

            Assert.Equal(medication.Id, response.Id);
            Assert.Equal(medication.Name, response.Name);
            Assert.Equal(medication.Quantity, response.Quantity);
            Assert.Equal(medication.CreationDate, response.CreationDate);
        }

    }
}
