using System.ComponentModel.DataAnnotations;
using api.Etc;
using api.Models.Dtos.Responses;
using api.Services;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
public class TotpController(ISecurityService securityService, MyDbContext ctx) : ControllerBase
{
    [HttpPost(nameof(TotpRegister))]
    public async Task<ActionResult<TotpRegisterResponseDto>> TotpRegister([FromBody] TotpRegisterRequestDto dto)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            var user = ctx.Users.First();
            user.TotpSecret = securityService.GenerateSecretKey();
            ctx.Users.Update(user);
            await ctx.SaveChangesAsync();
            var otpauthUrl =
                $"otpauth://totp/{Uri.EscapeDataString(nameof(StaticConstants.TickTickClone))}:{Uri.EscapeDataString(user.UserId)}?secret={user.TotpSecret}&issuer=" +
                nameof(StaticConstants.TickTickClone);


            return Ok(new TotpRegisterResponseDto
            {
                UserId = user.UserId,
                Message = "Scan the QR code with your authenticator app",
                QrCodeBase64 = securityService.GenerateQrCodeBase64(otpauthUrl),
                SecretKey = user.TotpSecret
            });
        }
        else
        {
            if (ctx.Users.Any(u => u.Email == dto.Email))
                throw new Exception("User already exists");
            var userId = Guid.NewGuid().ToString();
            var totpSecret = securityService.GenerateSecretKey();
            var user = new User(dto.Email, null, null, Role.User, totpSecret);
            ctx.Users.Add(user);
            await ctx.SaveChangesAsync();
            var otpauthUrl =
                $"otpauth://totp/{Uri.EscapeDataString(nameof(StaticConstants.TickTickClone))}:{Uri.EscapeDataString(userId)}?secret={totpSecret}&issuer=" +
                nameof(StaticConstants.TickTickClone);


            return Ok(new TotpRegisterResponseDto
            {
                UserId = userId,
                Message = "Scan the QR code with your authenticator app",
                QrCodeBase64 = securityService.GenerateQrCodeBase64(otpauthUrl),
                SecretKey = totpSecret
            });
        }
    }

    [HttpPost(nameof(TotpLogin))]
    public async Task<ActionResult<JwtResponse>> TotpLogin([FromBody] TotpLoginRequestDto request)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Email == request.Email) ??
                   throw new ValidationException("User not found");

        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);

        var token = securityService.GenerateJwt(user.UserId);
        return Ok(new JwtResponse
        {
            Jwt = token
        });
    }

    [HttpPost(nameof(TotpVerify))]
    public async Task<ActionResult> TotpVerify([FromBody] TotpVerifyRequestDto request)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.UserId == request.Id) ??
                   throw new Exception("User not found");

        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);
        return Ok();
    }

    [HttpPost(nameof(TotpRotate))]
    public async Task<ActionResult<TotpRegisterResponseDto>> TotpRotate(
        [FromBody] TotpRotateRequestDto request,
        [FromHeader] string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);

        var user = await ctx.Users.FirstOrDefaultAsync(u => u.UserId == jwt.Id);
        if (user == null)
            return NotFound(new { Error = "User not found" });

        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.CurrentTotpCode);

        var newTotpSecret = securityService.GenerateSecretKey();
        user.TotpSecret = newTotpSecret;
        await ctx.SaveChangesAsync();

        var otpauthUrl =
            $"otpauth://totp/{Uri.EscapeDataString(StaticConstants.TickTickClone)}:{Uri.EscapeDataString(user.UserId)}?secret={newTotpSecret}&issuer=" +
            nameof(StaticConstants.TickTickClone);

        return Ok(new TotpRegisterResponseDto
        {
            UserId = user.UserId,
            Message = "Scan the new QR code with your authenticator app",
            QrCodeBase64 = securityService.GenerateQrCodeBase64(otpauthUrl),
            SecretKey = newTotpSecret
        });
    }

    [HttpDelete(nameof(ToptUnregister))]
    public async Task<ActionResult> ToptUnregister([FromBody] TotpUnregisterRequestDto request,
        [FromHeader] string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.UserId == jwt.Id) ??
                   throw new Exception("User not found");
        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);

        ctx.Users.Remove(user);
        await ctx.SaveChangesAsync();

        return Ok();
    }
}