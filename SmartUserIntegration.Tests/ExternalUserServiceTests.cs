using Xunit;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartUserIntegration.ApiClient.Services;
using System.Net.Http.Json;
using System.Text;
using SmartUserIntegration.ApiClient.Models;

public class ExternalUserServiceTests
{
    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
    {
        var message = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"data\": {\"id\": 1, \"email\": \"test@reqres.in\", \"first_name\": \"Test\", \"last_name\": \"User\"}}", Encoding.UTF8, "application/json")
        };

        var handler = new MockHttpMessageHandler(message);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://reqres.in/api/")
        };

        var logger = new Mock<ILogger<ExternalUserService>>();
        var service = new ExternalUserService(client, logger.Object);

        var user = await service.GetUserByIdAsync(1);

        Assert.NotNull(user);
        Assert.Equal("Test User", user.FullName);
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _mockResponse;

        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            _mockResponse = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_mockResponse);
        }
    }
}
