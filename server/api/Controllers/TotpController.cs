using System.ComponentModel.DataAnnotations;
using api.Services;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class TotpController(ISecurityService securityService, MyDbContext ctx) : ControllerBase
{

    [HttpPost(nameof(TotpRegister))]
    public async Task<IActionResult> TotpRegister([FromBody] TotpRegisterRequestDto registerRequestDto)
    {
        if (ctx.Users.Any(u => u.Email == registerRequestDto.Email))
            return BadRequest(new { Error = "email already exists" });

        var totpSecret = securityService.GenerateSecretKey();

        var user = new User
        {
            Email = registerRequestDto.Email,
            TotpSecret = totpSecret,
            Role = nameof(User),
            UserId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var otpauthUrl =
            $"otpauth://totp/{Uri.EscapeDataString("YourApp")}:{Uri.EscapeDataString(user.UserId)}?secret={totpSecret}&issuer=YourApp";

        return Ok(new TotpRegisterResponseDto
        {
            Message = "Scan the QR code with your authenticator app",
            QrCodeBase64 = securityService.GenerateQrCodeBase64(otpauthUrl),
            SecretKey = totpSecret // For manual entry if needed
        });
    }

    [HttpPost(nameof(TotpLogin))]
    public async Task<IActionResult> TotpLogin([FromBody] TotpLoginRequestDto request)
    {
        var user = ctx.Users.FirstOrDefault(u => u.Email == request.Email) ?? throw new Exception("");

        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);
        var token = securityService.GenerateJwt(user.UserId);

        return Ok(token);


    }

    [HttpPost("rotate-totp")]

    public async Task<IActionResult> RotateTotpSecret([FromBody] RotateTotpRequestDto requestDto,
        [FromHeader] string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrow(authorization);
        var user = ctx.Users.FirstOrDefault(u => u.UserId == jwt.Id);

        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, requestDto.CurrentTotpCode);

        // Generate new TOTP secret
        var newTotpSecret = securityService.GenerateSecretKey();
        user.TotpSecret = newTotpSecret;
        ctx.Users.Update(user);
        ctx.SaveChanges();


        var otpauthUrl =
            $"otpauth://totp/{Uri.EscapeDataString("YourApp")}:{Uri.EscapeDataString(user.UserId)}?secret={newTotpSecret}&issuer=YourApp";

        return Ok(new TotpRegisterResponseDto 
        {
            Message = "Scan the new QR code with your authenticator app",
            QrCodeBase64 = securityService.GenerateQrCodeBase64(otpauthUrl),
            SecretKey = newTotpSecret
        });
    }

    [HttpPost("verify-setup")]
    public async Task<IActionResult> VerifyTotpSetup([FromBody] VerifySetupRequestDto requestDto)
    {
        var user = ctx.Users.FirstOrDefault(u => u.UserId == requestDto.UserId) ??
                   throw new Exception("Not found!");
        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, requestDto.TotpCode);
        return Ok();
    }

}

public class TotpRegisterResponseDto
{
    public string Message { get; set; }
    public string QrCodeBase64 { get; set; }
    public string SecretKey { get; set; }
}

public class TotpRegisterRequestDto
{
    [Required]
    public string Email { get; set; }
}

public class TotpLoginRequestDto
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string TotpCode { get; set; }
}

public class RotateTotpRequestDto
{
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string CurrentTotpCode { get; set; }

}

public class VerifySetupRequestDto
{
    [Required]
    public string UserId { get; set; }
    
    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string TotpCode { get; set; }
}