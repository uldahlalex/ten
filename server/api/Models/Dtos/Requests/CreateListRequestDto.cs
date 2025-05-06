using System.ComponentModel.DataAnnotations;

namespace api;

public class CreateListRequestDto
{
    [MinLength(1)] [Required]
    public string ListName { get; set; }
}