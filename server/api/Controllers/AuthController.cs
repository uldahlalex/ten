using System.ComponentModel.DataAnnotations;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Services;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class AuthController(ISecurityService securityService, MyDbContext ctx) : ControllerBase
{
    [Route(nameof(Register))]
    public Task<ActionResult<JwtResponse>> Register([FromBody] AuthRequestDto dto)
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
        var responseDto = new JwtResponse()
        {
            Jwt = securityService.GenerateJwt(uid)
        };

        return Task.FromResult<ActionResult<JwtResponse>>(Ok(responseDto));
    }

    [HttpPost]
    [Route(nameof(Login))]
    public Task<ActionResult<JwtResponse>> Login([FromBody] AuthRequestDto dto)
    {
        var user = ctx.Users.FirstOrDefault(u => u.Email == dto.Email);
        if (user == null)
            throw new ValidationException("User not found");
        if (user.PasswordHash != securityService.Hash(dto.Password + user.Salt))
            throw new ValidationException("Invalid password");
        var responseDto = new JwtResponse()
        {
            Jwt = securityService.GenerateJwt(user.UserId)
        };
        return Task.FromResult<ActionResult<JwtResponse>>(Ok(responseDto));
    }
}