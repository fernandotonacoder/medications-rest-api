using Medications.Api.Contracts;
using Medications.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace Medications.Unit.Tests
{
    public class CreateMedicationRequestUnitTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        public void Name_WhenEmptyOrWhitespace_IsInvalid(string name)
        {
            var results = Validate(new CreateMedicationRequest { Name = name, Quantity = 10 });

            var error = Assert.Single(results);
            Assert.Equal("The Name field is required.", error.ErrorMessage);
            Assert.Contains(nameof(CreateMedicationRequest.Name), error.MemberNames);
            Assert.DoesNotContain(nameof(CreateMedicationRequest.Quantity), error.MemberNames);
        }

        [Fact]
        public void Name_WhenNotSet_IsInvalid()
        {
            var results = Validate(new CreateMedicationRequest { Quantity = 10 });

            var error = Assert.Single(results);
            Assert.Equal("The Name field is required.", error.ErrorMessage);
            Assert.Contains(nameof(CreateMedicationRequest.Name), error.MemberNames);
        }

        [Fact]
        public void Name_WhenLongerThanMaxLength_IsInvalid()
        {
            var results = Validate(new CreateMedicationRequest
            {
                Name = new string('a', Medication.NameMaxLength + 1),
                Quantity = 10
            });

            var error = Assert.Single(results);
            Assert.Contains(nameof(CreateMedicationRequest.Name), error.MemberNames);
        }

        [Fact]
        public void Name_AtMaxLength_IsValid()
        {
            var results = Validate(new CreateMedicationRequest
            {
                Name = new string('a', Medication.NameMaxLength),
                Quantity = 10
            });

            Assert.Empty(results);
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
