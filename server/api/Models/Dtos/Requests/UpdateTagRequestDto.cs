using System.ComponentModel.DataAnnotations;

namespace api;

public class UpdateTagRequestDto
{
    [Required]
    public string TagId { get; set; }
    [Required]

    public string NewName { get; set; }
}