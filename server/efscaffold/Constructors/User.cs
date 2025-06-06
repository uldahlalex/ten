// ReSharper disable once CheckNamespace
namespace efscaffold.Entities;

public partial class User
{
    public User(
        string email, 
        string? salt, 
        string? passwordHash, 
        Role role, string? totpSecret,
        DateTime? createdAt = null,
        string? userId = null)
    {
        UserId = userId ?? Guid.NewGuid().ToString();
        Email = email;
        Salt = salt;
        PasswordHash = passwordHash;
        Role = role.GetDescription();
        CreatedAt = createdAt ?? DateTime.UtcNow;
        TotpSecret = totpSecret;
    }
}