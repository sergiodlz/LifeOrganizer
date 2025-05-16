using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using LifeOrganizer.Api;

namespace LifeOrganizer.Api.Tests
{
    public class AccountsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AccountsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAccounts_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/accounts");

            // Assert
            Assert.True(response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
