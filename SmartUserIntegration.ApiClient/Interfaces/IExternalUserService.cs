using SmartUserIntegration.ApiClient.Models;

namespace SmartUserIntegration.ApiClient.Interfaces;

public interface IExternalUserService
{
    Task<User> GetUserByIdAsync(int userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
}
