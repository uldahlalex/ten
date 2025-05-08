using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class JwtClaims
{
    [MinLength(1)][Required] public string Id { get; set; } = null!;
}