namespace Portifolio.Tests;
using Portifolio.Server.DTOs.Users;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class RegisterUsers : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public RegisterUsers(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task RegistrandoComSucesso()
    {
        TemplateUser dto = new()
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Test@123"
        };
        var response = await _client.PostAsJsonAsync("/api/User", dto, TestContext.Current.CancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Status: {(int)response.StatusCode}");
        Console.WriteLine($"Resposta da API: {responseBody}");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
