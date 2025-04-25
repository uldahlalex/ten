using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
public class AuthController(ISecurityService securityService, MyDbContext ctx) : ControllerBase
{
    public const string RegisterRoute = nameof(Register);

    public const string LoginRoute = nameof(Login);

    [Route(RegisterRoute)]
    public async Task<ActionResult<string>> Register([FromBody] AuthRequestDto dto)
    {
        if (ctx.Users.Any(u => u.Email == dto.Email))
            throw new Exception("Cannot register with email");

        var salt = Guid.NewGuid().ToString();
        var uid = Guid.NewGuid().ToString();

        ctx.Users.Add(new User
        {
            Email = dto.Email,
            PasswordHash = securityService.Hash(dto.Password + salt),
            Role = nameof(User),
            Salt = salt,
            UserId = uid
        });
        ctx.SaveChanges();

        return Ok(securityService.GenerateJwt(uid));
    }

    [HttpPost]
    [Route(LoginRoute)]
    public async Task<ActionResult<string>> Login([FromBody] AuthRequestDto dto)
    {
        var user = ctx.Users.FirstOrDefault(u => u.Email == dto.Email);
        if (user == null)
            throw new Exception("User not found");
        if (user.PasswordHash != securityService.Hash(dto.Password + user.Salt))
            throw new Exception("Invalid password");
        return Ok(securityService.GenerateJwt(user.UserId));
    }
}

public class AuthRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}