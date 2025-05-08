using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

public class UpdateTagRequestDto
{
    [Required] public string TagId { get; set; }= null!;

    [Required] public string NewName { get; set; }= null!;
}