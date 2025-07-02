using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class UserDataService(MyDbContext ctx, TimeProvider timeProvider) : IUserDataService
{
    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await ctx.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await ctx.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateUserAsync(string email, string salt, string passwordHash, Role role, string? totpSecret = null)
    {
        var userId = Guid.NewGuid().ToString();
        var timestamp = timeProvider.GetUtcNow().UtcDateTime;
        
        var user = new User(timestamp, email, salt, passwordHash, role, totpSecret);
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();
        
        return user;
    }

    public async Task<bool> UserExistsAsync(string userId)
    {
        return await ctx.Users.AnyAsync(u => u.UserId == userId);
    }

    public async Task<bool> UserExistsByEmailAsync(string email)
    {
        return await ctx.Users.AnyAsync(u => u.Email == email);
    }

    public async Task UpdateUserAsync(User user)
    {
        ctx.Users.Update(user);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(User user)
    {
        ctx.Users.Remove(user);
        await ctx.SaveChangesAsync();
    }
}