using System.ComponentModel.DataAnnotations;
using api.Services;
using efscaffold;
using efscaffold.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class AuthController(ISecurityService securityService, MyDbContext ctx) : ControllerBase
{
    [Route(nameof(Register))]
    public async Task<ActionResult<string>> Register([FromBody] AuthRequestDto dto)
    {
        if (ctx.Users.Any(u => u.Email == dto.Email))
            throw new ValidationException("Cannot register with email");

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
    [Route(nameof(Login))]
    public async Task<ActionResult<string>> Login([FromBody] AuthRequestDto dto)
    {
        var user = ctx.Users.FirstOrDefault(u => u.Email == dto.Email);
        if (user == null)
            throw new ValidationException("User not found");
        if (user.PasswordHash != securityService.Hash(dto.Password + user.Salt))
            throw new ValidationException("Invalid password");
        return Ok(securityService.GenerateJwt(user.UserId));
    }
}

public class AuthRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}