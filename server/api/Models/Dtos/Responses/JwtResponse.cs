using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Responses;

public class JwtResponse
{
    [Required] [MinLength(1)] public string Jwt { get; set; }
}