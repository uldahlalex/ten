using System.ComponentModel.DataAnnotations;

// ReSharper disable InconsistentNaming

namespace api.Models;

public sealed class AppOptions
{
    [Required][MinLength(20)] public string JwtSecret { get; set; } = string.Empty!;
    [Required][MinLength(20)] public string DbConnectionString { get; set; } = string.Empty!;
    public string RunsOn { get; set; } = string.Empty!;
}