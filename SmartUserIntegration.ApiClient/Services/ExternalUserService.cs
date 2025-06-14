using Microsoft.Extensions.Logging;
using System.Text.Json;
using SmartUserIntegration.ApiClient.Models;
using SmartUserIntegration.ApiClient.Interfaces;

namespace SmartUserIntegration.ApiClient.Services;

public class ExternalUserService : IExternalUserService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalUserService> _logger;

    public ExternalUserService(HttpClient httpClient, ILogger<ExternalUserService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync("users");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("User {UserId} not found. Status: {StatusCode}", userId, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var userJson = doc.RootElement.GetProperty("data").GetRawText();
            var dto = JsonSerializer.Deserialize<UserDto>(userJson);

            return new User
            {
                Id = dto.Id,
                Email = dto.Email,
                FullName = $"{dto.First_Name} {dto.Last_Name}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var users = new List<User>();
        int page = 1;

        while (true)
        {
            var response = await _httpClient.GetAsync("users");
            if (!response.IsSuccessStatusCode) break;

            var content = await response.Content.ReadAsStringAsync();
            var listDto = JsonSerializer.Deserialize<UserListDto>(content);
            if (listDto == null || listDto.Data == null) break;

            users.AddRange(listDto.Data.Select(dto => new User
            {
                Id = dto.Id,
                Email = dto.Email,
                FullName = $"{dto.First_Name} {dto.Last_Name}"
            }));

            if (page >= listDto.Total_Pages) break;
            page++;
        }

        return users;
    }
}
