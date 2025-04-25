using efscaffold.Entities;
using Microsoft.VisualBasic;

namespace api.Mappers;

public static class TaskMappingExtensions
{
    public static Tickticktask ToEntity(this CreateTaskRequestDto dto, List<TaskTag> tags, Tasklist list)
    {
        var task = new Tickticktask()
        {
            TaskId = dto.ListId,
            ListId = dto.ListId,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Priority = dto.Priority,
            CreatedAt = DateTime.UtcNow,
            Completed = false,
            CompletedAt = DateTime.MinValue,
            List = list,
            TaskTags = tags,
        };
        return task;
    }
}