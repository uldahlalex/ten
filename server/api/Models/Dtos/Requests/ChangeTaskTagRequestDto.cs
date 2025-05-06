using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

public class ChangeTaskTagRequestDto
{
    [Required] public string TagId { get; set; }

    [Required] public string TaskId { get; set; }
}