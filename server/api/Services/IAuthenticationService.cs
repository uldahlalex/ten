using api.Controllers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;

namespace api.Services;

public interface IAuthenticationService
{
    Task<JwtResponse> Register(AuthRequestDto dto);
    Task<JwtResponse> Login(AuthRequestDto dto);
    Task<TotpRegisterResponseDto> RegisterTotp(TotpRegisterRequestDto dto);
    Task<JwtResponse> TotpLogin(TotpLoginRequestDto request);
    Task TotpVerify(TotpVerifyRequestDto request);
    Task<TotpRegisterResponseDto> TotpRotate(TotpRotateRequestDto request, string authorization);
    Task TotpUnregister(TotpUnregisterRequestDto request, string authorization);
}