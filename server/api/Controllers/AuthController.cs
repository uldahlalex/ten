using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
public class AuthController(ISecurityService securityService, MyDbContext dbContext) : ControllerBase
{

    public const string RegisterRoute = nameof(Register);
    [Route(RegisterRoute)]
    public async Task<ActionResult<string>> Register([FromBody] AuthRequestDto dto)
    {
        if (dbContext.Users.Any(u => u.Email == dto.Email))
            throw new Exception("Cannot register with email");
        
        var salt = Guid.NewGuid().ToString();
        var uid = Guid.NewGuid().ToString();

        dbContext.Users.Add(new User
        {
            Email = dto.Email,
            PasswordHash = securityService.Hash(dto.Password + salt),
            Role = nameof(User),
            Salt = salt,
            UserId = uid
        });
        dbContext.SaveChanges();
        
        return Ok(securityService.GenerateJwt(uid));
    }
}

public class AuthRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}