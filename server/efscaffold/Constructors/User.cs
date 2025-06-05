// ReSharper disable once CheckNamespace
namespace efscaffold.Entities;

public partial class User
{
    public User(
        string email, 
        string? salt, 
        string? passwordHash, 
        string role, string? totpSecret,
        DateTime? createdAt = null,
        string? userId = null)
    {
        UserId = userId ?? Guid.NewGuid().ToString();
        Email = email;
        Salt = salt;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        TotpSecret = totpSecret;
    }
}