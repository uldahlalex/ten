using System.ComponentModel.DataAnnotations;

namespace api;

public class ChangeTaskTagRequestDto
{
    [Required]
    public string TagId { get; set; }
    [Required]
    public string TaskId { get; set; }
}