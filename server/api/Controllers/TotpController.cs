using System.ComponentModel.DataAnnotations;
using api.Etc;
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
    public async Task<ActionResult<TotpRegisterResponseDto>> TotpRegister()
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            var user = ctx.Users.First();
            user.TotpSecret = securityService.GenerateSecretKey();
            ctx.Users.Update(user);
            await ctx.SaveChangesAsync();
            var otpauthUrl =
                $"otpauth://totp/{Uri.EscapeDataString("YourApp")}:{Uri.EscapeDataString(user.UserId)}?secret={user.TotpSecret}&issuer=YourApp";


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
            var userId = Guid.NewGuid().ToString();
            var totpSecret = securityService.GenerateSecretKey();
            var user = new User
            {
                UserId = userId,
                TotpSecret = totpSecret,
                Role = nameof(User),
                CreatedAt = DateTime.UtcNow,
                Email = "test@totpuser.dk"
            };
            ctx.Users.Add(user);
            await ctx.SaveChangesAsync();
            var otpauthUrl =
                $"otpauth://totp/{Uri.EscapeDataString("YourApp")}:{Uri.EscapeDataString(userId)}?secret={totpSecret}&issuer=YourApp";


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
    public async Task<ActionResult<string>> TotpLogin([FromBody] TotpLoginRequestDto request)
    {
        
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.TotpSecret == request.TotpCode) ??
                   throw new ValidationException("User not found");

        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);

        var token = securityService.GenerateJwt(user.UserId);
        return Ok(token);
    }

    [HttpPost(nameof(TotpVerify))]
    public async Task<ActionResult> TotpVerify([FromBody] TotpVerifyRequestDto request)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId) ??
                   throw new Exception("User not found");

        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);
        return Ok();
    }

    [HttpPost(nameof(TotpRotate))]
    public async Task<ActionResult<TotpRegisterResponseDto>> TotpRotate([FromBody] TotpRotateRequestDto request,
        [FromHeader] string authorization)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);


        var jwt = securityService.VerifyJwtOrThrow(authorization);

        var user = await ctx.Users.FirstOrDefaultAsync(u => u.UserId == jwt.Id);
        if (user == null)
            return NotFound(new { Error = "User not found" });

        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.CurrentTotpCode);

        var newTotpSecret = securityService.GenerateSecretKey();
        user.TotpSecret = newTotpSecret;
        await ctx.SaveChangesAsync();

        var otpauthUrl =
            $"otpauth://totp/{Uri.EscapeDataString(StaticConstants.TickTickClone)}:{Uri.EscapeDataString(user.UserId)}?secret={newTotpSecret}&issuer=YourApp";

        return Ok(new TotpRegisterResponseDto
        {
            UserId = user.UserId,
            Message = "Scan the new QR code with your authenticator app",
            QrCodeBase64 = securityService.GenerateQrCodeBase64(otpauthUrl),
            SecretKey = newTotpSecret
        });
    }

    [HttpDelete(nameof(ToptUnregister))]
    public async Task<IActionResult> ToptUnregister([FromBody] TotpUnregisterRequestDto request)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId) ??
                   throw new Exception("User not found");
        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);

        ctx.Users.Remove(user);
        await ctx.SaveChangesAsync();

        return Ok(new { Message = "Device unregistered successfully" });
    }
}