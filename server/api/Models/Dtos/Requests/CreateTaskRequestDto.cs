using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     Task is always created for the user sending the request
/// </summary>
public record CreateTaskRequestDto(
    string ListId,           // Required (non-nullable)
    [MinLength(1)]string Title,            // Required (non-nullable)
    [MinLength(1)]string Description,      // Required (non-nullable)
    /// <summary>
    ///     Due date is optional since tasks may have none
    /// </summary>
    DateTime? DueDate,       // Optional (nullable)
    [Range(1,5)]int Priority,            // Required (non-nullable value type)
    /// <summary>
    ///     List of tag IDs to add to the task when it is created
    /// </summary>
    ICollection<string> TagsIds  // Required (non-nullable reference type)
);