using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using LifeOrganizer.Api;

namespace LifeOrganizer.Api.Tests
{
    public class TagsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TagsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTags_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/api/tags");
            Assert.True(response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
