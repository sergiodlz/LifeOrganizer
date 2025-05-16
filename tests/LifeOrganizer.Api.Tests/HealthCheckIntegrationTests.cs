using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using LifeOrganizer.Api;

namespace LifeOrganizer.Api.Tests
{
    public class HealthCheckIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public HealthCheckIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task HealthEndpoint_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/health");
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}
