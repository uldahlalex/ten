using System.ComponentModel.DataAnnotations;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Services;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    [Route(nameof(Register))]
    public Task<ActionResult<JwtResponse>> Register([FromBody] AuthRequestDto dto)
    {
        var responseDto = authenticationService.Register(dto);

        return Task.FromResult<ActionResult<JwtResponse>>(Ok(responseDto));
    }



    [HttpPost]
    [Route(nameof(Login))]
    public Task<ActionResult<JwtResponse>> Login([FromBody] AuthRequestDto dto)
    {
        var responseDto =  authenticationService.Login(dto);
        return Task.FromResult<ActionResult<JwtResponse>>(Ok(responseDto));
    }

  
}