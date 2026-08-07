using Medications.Api.Data;
using Medications.Api.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Medications.Unit.Tests
{
    public class MedicationEndpointsRegistrationUnitTests
    {
        [Theory]
        [InlineData("GET", "/api/medications/")]
        [InlineData("GET", "/api/medications/{id}")]
        [InlineData("POST", "/api/medications/")]
        [InlineData("DELETE", "/api/medications/{id}")]
        public void MapMedicationEndpoints_RegistersRoute(string httpMethod, string pattern)
        {
            var endpoints = MapEndpoints();

            Assert.Contains(endpoints, endpoint =>
                endpoint.RoutePattern.RawText == pattern
                && endpoint.Metadata.GetMetadata<HttpMethodMetadata>()!.HttpMethods.Contains(httpMethod));
        }

        [Fact]
        public void MapMedicationEndpoints_RegistersOnlyTheFourEndpoints()
        {
            Assert.Equal(4, MapEndpoints().Count);
        }

        [Fact]
        public void GetMedicationById_IsNamed_SoCreatedAtRouteResolvesTheLocationHeader()
        {
            var endpoints = MapEndpoints();

            var named = Assert.Single(endpoints, endpoint =>
                endpoint.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName == "GetMedication");
            Assert.Equal("/api/medications/{id}", named.RoutePattern.RawText);
        }

        private static List<RouteEndpoint> MapEndpoints()
        {
            var builder = WebApplication.CreateSlimBuilder();
            builder.Services.AddDbContext<MedicationsDbContext>(
                options => options.UseInMemoryDatabase(nameof(MedicationEndpointsRegistrationUnitTests)));
            builder.Services.AddSingleton(TimeProvider.System);

            var app = builder.Build();

            app.MapMedicationEndpoints();

            return ((IEndpointRouteBuilder)app).DataSources
                .SelectMany(dataSource => dataSource.Endpoints)
                .OfType<RouteEndpoint>()
                .ToList();
        }
    }
}
