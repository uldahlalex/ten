using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

public class TotpRegisterResponseDto
{
    [Required]
    public string Message { get; set; }
    [Required]
    public string QrCodeBase64 { get; set; }
    [Required]
    public string SecretKey { get; set; }
    [Required]
    public string UserId { get; set; }
}