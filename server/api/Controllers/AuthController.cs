using System.ComponentModel.DataAnnotations;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Services;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    [Route(nameof(Register))]
    public async Task<ActionResult<JwtResponse>> Register([FromBody] AuthRequestDto dto)
    {
        var responseDto = await authenticationService.Register(dto);

        return responseDto;
    }


    [AllowAnonymous]
    [HttpPost]
    [Route(nameof(Login))]
    public async Task<ActionResult<JwtResponse>> Login([FromBody] AuthRequestDto dto)
    {
        var responseDto = await authenticationService.Login(dto);
        return responseDto;
    }

  
}