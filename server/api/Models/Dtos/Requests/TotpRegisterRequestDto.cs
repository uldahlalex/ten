using System.ComponentModel.DataAnnotations;

namespace api.Controllers;

public class TotpRegisterRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}