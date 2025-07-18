using System.ComponentModel.DataAnnotations;
using api.Etc;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Services;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
public class TotpController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost(nameof(TotpRegister))]
    public async Task<ActionResult<TotpRegisterResponseDto>> TotpRegister([FromBody] TotpRegisterRequestDto dto)
    {
        return  Ok(await authenticationService.RegisterTotp(dto));
    }

    

    [HttpPost(nameof(TotpLogin))]
    public async Task<ActionResult<JwtResponse>> TotpLogin([FromBody] TotpLoginRequestDto request)
    {
        var result = await authenticationService.TotpLogin(request);
        return Ok(result);
    }

    [HttpPost(nameof(TotpVerify))]
    public async Task<ActionResult> TotpVerify([FromBody] TotpVerifyRequestDto request)
    {
        await authenticationService.TotpVerify(request);
        return Ok();
    }

    [HttpPost(nameof(TotpRotate))]
    public async Task<ActionResult<TotpRegisterResponseDto>> TotpRotate(
        [FromBody] TotpRotateRequestDto request,
        [FromHeader] string authorization)
    {
       var result = await authenticationService.TotpRotate(request, authorization);
       return Ok(result);
    }

    [HttpDelete(nameof(ToptUnregister))]
    public async Task<ActionResult> ToptUnregister([FromBody] TotpUnregisterRequestDto request,
        [FromHeader] string authorization)
    {
        await authenticationService.TotpUnregister(request, authorization);

        return Ok();
    }
}