using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class JwtClaims
{
    [MinLength(1)] public string Id { get; set; }
}