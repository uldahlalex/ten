// ReSharper disable once CheckNamespace
namespace efscaffold.Entities;

public partial class User
{
    /// <summary>
    /// Private parameterless constructor to be used by EF Core.
    /// </summary>
    private User ()
    {
        
    }
    
    public User(DateTime createdAt,
        string email, 
        string? salt, 
        string? passwordHash, 
        Role role, string? totpSecret,
        string? userId = null)
    {
        UserId = userId ?? Guid.NewGuid().ToString();
        Email = email;
        Salt = salt;
        PasswordHash = passwordHash;
        Role = role.GetDescription();
        CreatedAt = createdAt;
        TotpSecret = totpSecret;
    }
}