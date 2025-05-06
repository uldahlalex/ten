using System.ComponentModel.DataAnnotations;

namespace api;

public class CreateTagRequestDto
{
    [Required]
    public string TagName { get; set; }
}