using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

public class AuthRequestDto
{
    [Required] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}