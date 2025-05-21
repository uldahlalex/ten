using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Replaces all of the properties with the values. Nulls are not allowed, since the client app should send the
///     existing object and not just declare certain properties to replace
/// </summary>
public class UpdateTaskRequestDto
{
    public UpdateTaskRequestDto(string id, string listId, bool completed, string title, string description,
        DateTime? dueDate, int priority)
    {
        Id = id;
        ListId = listId;
        Completed = completed;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
    }

    [Required] public string Id { get; set; } = null!;

    [Required] public string ListId { get; set; } = null!;

    [Required] public bool Completed { get; set; }

    [MinLength(1)] [Required] public string Title { get; set; } = null!;

    [MinLength(1)] [Required] public string Description { get; set; } = null!;

    /// <summary>
    ///     Due date can be "removed" by assigning it null
    /// </summary>
    public DateTime? DueDate { get; set; }

    [Range(1, 5)] [Required] public int Priority { get; set; }
}