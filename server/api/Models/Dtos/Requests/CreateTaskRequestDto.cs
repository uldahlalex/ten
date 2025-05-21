using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
/// Task is always created for the user sending the request
/// </summary>
public class CreateTaskRequestDto
{
    public CreateTaskRequestDto(string listId, string title, string description, DateTime? dueDate, int priority)
    {
        ListId = listId;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
    }

    [Required] public string ListId { get; set; } = null!;

    [MinLength(1)] [Required] public string Title { get; set; } = null!;

    [MinLength(1)] [Required] public string Description { get; set; } = null!;

    /// <summary>
    /// Due date is optional since tasks may have none
    /// </summary>
    public DateTime? DueDate { get; set; }

    [Range(1, 5)] [Required] public int Priority { get; set; }


    /// <summary>
    /// List of tag IDs to add to the task when it is created
    /// </summary>
    public virtual ICollection<string> TagsIds { get; set; } = new List<string>();
}