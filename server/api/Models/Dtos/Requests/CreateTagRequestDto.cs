using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

public class CreateTagRequestDto
{
    [Required] public string TagName { get; set; }= null!;
}