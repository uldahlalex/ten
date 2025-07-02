using System.ComponentModel.DataAnnotations;
using api.Controllers;
using api.Etc;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public interface IAuthenticationService
{
    JwtResponse Register(AuthRequestDto dto);
    JwtResponse Login(AuthRequestDto dto);
    Task<TotpRegisterResponseDto> RegisterTotp(TotpRegisterRequestDto dto);
    Task<JwtResponse> TotpLogin(TotpLoginRequestDto request);
    Task TotpVerify(TotpVerifyRequestDto request);
    Task<TotpRegisterResponseDto> TotpRotate(TotpRotateRequestDto request, string authorization);
    Task TotpUnregister(TotpUnregisterRequestDto request, string authorization);
}

public class AuthenticationService(MyDbContext ctx, TimeProvider timeProvider, ISecurityService securityService) : IAuthenticationService
{
    public JwtResponse Register(AuthRequestDto dto)
    {
        if (ctx.Users.Any(u => u.Email == dto.Email))
            throw new ValidationException("Cannot register with email");

        var salt = Guid.NewGuid().ToString();
        var uid = Guid.NewGuid().ToString();

        ctx.Users.Add(new User(timeProvider.GetUtcNow().UtcDateTime, dto.Email, salt, securityService.Hash(dto.Password + salt), Role.User, null ));
        ctx.SaveChanges();
        var responseDto = new JwtResponse
        {
            Jwt = securityService.GenerateJwt(uid)
        };
        return responseDto;
    }
    
    public JwtResponse Login(AuthRequestDto dto)
    {
        var user = ctx.Users.FirstOrDefault(u => u.Email == dto.Email);
        if (user == null)
            throw new ValidationException("User not found");
        if (user.PasswordHash != securityService.Hash(dto.Password + user.Salt))
            throw new ValidationException("Invalid password");
        var responseDto = new JwtResponse
        {
            Jwt = securityService.GenerateJwt(user.UserId)
        };
        return responseDto;
    }
    
    public async Task<TotpRegisterResponseDto> RegisterTotp(TotpRegisterRequestDto dto)
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


            return (new TotpRegisterResponseDto
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
            var timestamp = timeProvider.GetUtcNow().UtcDateTime;
            var user = new User(timestamp,dto.Email, null, null, Role.User, totpSecret);
            ctx.Users.Add(user);
            await ctx.SaveChangesAsync();
            var otpauthUrl =
                $"otpauth://totp/{Uri.EscapeDataString(nameof(StaticConstants.TickTickClone))}:{Uri.EscapeDataString(userId)}?secret={totpSecret}&issuer=" +
                nameof(StaticConstants.TickTickClone);


            return (new TotpRegisterResponseDto
            {
                UserId = userId,
                Message = "Scan the QR code with your authenticator app",
                QrCodeBase64 = securityService.GenerateQrCodeBase64(otpauthUrl),
                SecretKey = totpSecret
            });
        }
    }


    public async Task TotpVerify(TotpVerifyRequestDto request)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.UserId == request.Id) ??
                   throw new Exception("User not found");

        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);    }

    public async Task<TotpRegisterResponseDto> TotpRotate(TotpRotateRequestDto request, string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrowReturnClaims(authorization);

        var user = await ctx.Users.FirstOrDefaultAsync(u => u.UserId == jwt.Id);
        if (user == null)
            throw new Exception("User not found");

        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.CurrentTotpCode);

        var newTotpSecret = securityService.GenerateSecretKey();
        user.TotpSecret = newTotpSecret;
        await ctx.SaveChangesAsync();

        var otpauthUrl =
            $"otpauth://totp/{Uri.EscapeDataString(StaticConstants.TickTickClone)}:{Uri.EscapeDataString(user.UserId)}?secret={newTotpSecret}&issuer=" +
            nameof(StaticConstants.TickTickClone);

        return (new TotpRegisterResponseDto
        {
            UserId = user.UserId,
            Message = "Scan the new QR code with your authenticator app",
            QrCodeBase64 = securityService.GenerateQrCodeBase64(otpauthUrl),
            SecretKey = newTotpSecret
        });
    }

    public async Task TotpUnregister(TotpUnregisterRequestDto request, string authorization)
    {
        var jwt = securityService.VerifyJwtOrThrowReturnClaims(authorization);
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.UserId == jwt.Id) ??
                   throw new Exception("User not found");
        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);

        ctx.Users.Remove(user);
        await ctx.SaveChangesAsync();
    }

    public async Task<JwtResponse> TotpLogin(TotpLoginRequestDto request)
    {
        var user =  await ctx.Users.FirstOrDefaultAsync(u => u.Email == request.Email) ??
                   throw new ValidationException("User not found");

        securityService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);

        var token = securityService.GenerateJwt(user.UserId);
        return new JwtResponse
        {
            Jwt = token
        };
    }
}