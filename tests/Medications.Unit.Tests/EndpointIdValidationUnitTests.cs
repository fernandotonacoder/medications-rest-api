using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Medications.Api.Endpoints;

namespace Medications.Unit.Tests
{
    public class EndpointIdValidationUnitTests
    {
        [Theory]
        [InlineData(nameof(MedicationEndpoints.GetMedication))]
        [InlineData(nameof(MedicationEndpoints.DeleteMedication))]
        public void IdParameter_DeclaresRangeStartingAtOne(string methodName)
        {
            var method = typeof(MedicationEndpoints).GetMethod(
                methodName, BindingFlags.NonPublic | BindingFlags.Static);

            Assert.NotNull(method);
            var idParameter = Assert.Single(method.GetParameters(), parameter => parameter.Name == "id");

            var range = Assert.IsType<RangeAttribute>(
                Assert.Single(idParameter.GetCustomAttributes(typeof(RangeAttribute), inherit: false)));
            Assert.Equal(1, range.Minimum);
            Assert.Equal("Id must be greater than zero.", range.ErrorMessage);
        }
    }
}
