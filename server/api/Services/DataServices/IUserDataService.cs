using efscaffold.Entities;

namespace api.Services;

public interface IUserDataService
{
    Task<User?> GetUserByIdAsync(string userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(string email, string salt, string passwordHash, Role role, string? totpSecret = null);
    Task<bool> UserExistsAsync(string userId);
    Task<bool> UserExistsByEmailAsync(string email);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
}