using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Responses;

public record TotpRegisterResponseDto(
    [Required] string Message,
    [Required] string QrCodeBase64,
    [Required] string SecretKey,
    [Required] string UserId
);