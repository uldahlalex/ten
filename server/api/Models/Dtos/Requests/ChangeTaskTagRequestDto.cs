using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Used for assigning and de-assigning tags to tasks. Works as "toggle", so if the tag already exists it is removed
///     and vice versa.
/// </summary>
public class ChangeTaskTagRequestDto
{
    public ChangeTaskTagRequestDto(string tagId, string taskId)
    {
        TagId = tagId;
        TaskId = taskId;
    }

    [Required] public string TagId { get; set; } = null!;

    [Required] public string TaskId { get; set; } = null!;
}