using System.ComponentModel.DataAnnotations;
using api.Controllers;
using api.Etc;
using api.Models;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using efscaffold.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace api.Services;

public class AuthenticationService(
    IUserDataService userDataService, 
    IOptionsMonitor<AppOptions> optionsMonitor,
    ICryptographyService cryptographyService, 
    IJwtService jwtService, 
    ITotpService totpService) : IAuthenticationService
{
    public async Task<JwtResponse> Register(AuthRequestDto dto)
    {
        if (await userDataService.UserExistsByEmailAsync(dto.Email))
            throw new ValidationException("Cannot register with email");

        var salt = Guid.NewGuid().ToString();
        var passwordHash = cryptographyService.Hash(dto.Password + salt);
        
        var user = await userDataService.CreateUserAsync(dto.Email, salt, passwordHash, Role.User);
        
        var responseDto = new JwtResponse
        {
            Jwt = jwtService.GenerateJwt(user.UserId, optionsMonitor.CurrentValue.JwtSecret)
        };
        return responseDto;
    }
    
    public async Task<JwtResponse> Login(AuthRequestDto dto)
    {
        var user = await userDataService.GetUserByEmailAsync(dto.Email);
        if (user == null)
            throw new ValidationException("User not found");
        if (user.PasswordHash != cryptographyService.Hash(dto.Password + user.Salt))
            throw new ValidationException("Invalid password");
        var responseDto = new JwtResponse
        {
            Jwt = jwtService.GenerateJwt(user.UserId, optionsMonitor.CurrentValue.JwtSecret)
        };
        return responseDto;
    }
    
    public async Task<TotpRegisterResponseDto> RegisterTotp(TotpRegisterRequestDto dto)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            var user = await userDataService.GetUserByEmailAsync(dto.Email) ?? throw new Exception("No development user found");
            user.TotpSecret = totpService.GenerateSecretKey();
            await userDataService.UpdateUserAsync(user);
            var otpauthUrl =
                $"otpauth://totp/{Uri.EscapeDataString(nameof(StaticConstants.TickTickClone))}:{Uri.EscapeDataString(user.UserId)}?secret={user.TotpSecret}&issuer=" +
                nameof(StaticConstants.TickTickClone);


            return (new TotpRegisterResponseDto
            {
                UserId = user.UserId,
                Message = "Scan the QR code with your authenticator app",
                QrCodeBase64 = totpService.GenerateQrCodeBase64(otpauthUrl),
                SecretKey = user.TotpSecret
            });
        }
        else
        {
            if (await userDataService.UserExistsByEmailAsync(dto.Email))
                throw new Exception("User already exists");
                
            var totpSecret = totpService.GenerateSecretKey();
            var user = await userDataService.CreateUserAsync(dto.Email, null, null, Role.User, totpSecret);
            var otpauthUrl =
                $"otpauth://totp/{Uri.EscapeDataString(nameof(StaticConstants.TickTickClone))}:{Uri.EscapeDataString(user.UserId)}?secret={totpSecret}&issuer=" +
                nameof(StaticConstants.TickTickClone);


            return (new TotpRegisterResponseDto
            {
                UserId = user.UserId,
                Message = "Scan the QR code with your authenticator app",
                QrCodeBase64 = totpService.GenerateQrCodeBase64(otpauthUrl),
                SecretKey = totpSecret
            });
        }
    }


    public async Task TotpVerify(TotpVerifyRequestDto request)
    {
        var user = await userDataService.GetUserByIdAsync(request.Id) ??
                   throw new Exception("User not found");

        totpService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);    }

    public async Task<TotpRegisterResponseDto> TotpRotate(TotpRotateRequestDto request, string authorization)
    {
        var jwt = jwtService.VerifyJwt(authorization);

        var user = await userDataService.GetUserByIdAsync(jwt.Id) ??
            throw new Exception("User not found");

        totpService.ValidateTotpCodeOrThrow(user.TotpSecret, request.CurrentTotpCode);

        var newTotpSecret = totpService.GenerateSecretKey();
        user.TotpSecret = newTotpSecret;
        await userDataService.UpdateUserAsync(user);

        var otpauthUrl =
            $"otpauth://totp/{Uri.EscapeDataString(StaticConstants.TickTickClone)}:{Uri.EscapeDataString(user.UserId)}?secret={newTotpSecret}&issuer=" +
            nameof(StaticConstants.TickTickClone);

        return (new TotpRegisterResponseDto
        {
            UserId = user.UserId,
            Message = "Scan the new QR code with your authenticator app",
            QrCodeBase64 = totpService.GenerateQrCodeBase64(otpauthUrl),
            SecretKey = newTotpSecret
        });
    }

    public async Task TotpUnregister(TotpUnregisterRequestDto request, string authorization)
    {
        var jwt = jwtService.VerifyJwt(authorization);
        var user = await userDataService.GetUserByIdAsync(jwt.Id) ??
                   throw new Exception("User not found");
        totpService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);

        await userDataService.DeleteUserAsync(user);
    }

    public async Task<JwtResponse> TotpLogin(TotpLoginRequestDto request)
    {
        var user = await userDataService.GetUserByEmailAsync(request.Email) ??
                   throw new ValidationException("User not found");

        totpService.ValidateTotpCodeOrThrow(user.TotpSecret, request.TotpCode);

        var token = jwtService.GenerateJwt(user.UserId, optionsMonitor.CurrentValue.JwtSecret);
        return new JwtResponse
        {
            Jwt = token
        };
    }
}