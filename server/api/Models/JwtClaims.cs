using System.ComponentModel.DataAnnotations;

namespace api;

public class JwtClaims
{
    [MinLength(1)]
    public string Id { get; set; }
}