using System.ComponentModel.DataAnnotations;

// ReSharper disable InconsistentNaming

namespace api.Models;

public sealed class AppOptions
{
    [Required] public string JwtSecret { get; set; } = string.Empty!;
    [Required] public string DbConnectionString { get; set; } = string.Empty!;
    public string RunsOn { get; set; } = string.Empty!;
}