using Medications.Api.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Medications.Unit.Tests
{
    public class CreateMedicationRequestUnitTests
    {
        [Fact]
        public void Name_WhenEmptyString_IsInvalid()
        {
            var results = Validate(new CreateMedicationRequest { Name = "", Quantity = 10 });

            var error = Assert.Single(results);
            Assert.Equal("The Name field is required.", error.ErrorMessage);
            Assert.Contains(nameof(CreateMedicationRequest.Name), error.MemberNames);
            Assert.DoesNotContain(nameof(CreateMedicationRequest.Quantity), error.MemberNames);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Quantity_WhenNotGreaterThanZero_IsInvalid(int quantity)
        {
            var results = Validate(new CreateMedicationRequest { Name = "Aspirin", Quantity = quantity });

            var error = Assert.Single(results);
            Assert.Equal("Quantity must be greater than 0", error.ErrorMessage);
            Assert.DoesNotContain(nameof(CreateMedicationRequest.Name), error.MemberNames);
            Assert.Contains(nameof(CreateMedicationRequest.Quantity), error.MemberNames);
        }

        private static List<ValidationResult> Validate(CreateMedicationRequest request)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(
                request,
                new ValidationContext(request),
                results,
                validateAllProperties: true);
            return results;
        }
    }
}
