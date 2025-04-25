using efscaffold.Entities;

namespace api.Mappers;

public static class TaskMappingExtensions
{
    public static Tickticktask ToEntity(this CreateTaskRequestDto dto, List<TaskTag> tags, Tasklist list)
    {
        var task = new Tickticktask
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
            TaskTags = tags
        };
        return task;
    }

    public static TasklistDto ToDto(this Tasklist tasklist)
    {
        var dto = new TasklistDto
        {
            ListId = tasklist.ListId,
            UserId = tasklist.UserId,
            Name = tasklist.Name,
            CreatedAt = tasklist.CreatedAt,
            Tickticktasks = tasklist.Tickticktasks.Select(t => t.ToDto()).ToList(),
            User = tasklist.User.ToDto()
        };
        return dto;
    }

    public static UserDto ToDto(this User user)
    {
        var dto = new UserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            Salt = user.Salt,
            PasswordHash = user.PasswordHash,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            Tags = user.Tags.Select(t => t.ToDto()).ToList(),
            Tasklists = user.Tasklists.Select(t => t.ToDto()).ToList()
        };
        return dto;
    }

    public static TickticktaskDto ToDto(this Tickticktask tickticktask)
    {
        var dto = new TickticktaskDto
        {
            TaskId = tickticktask.TaskId,
            ListId = tickticktask.ListId,
            Title = tickticktask.Title,
            Description = tickticktask.Description,
            DueDate = tickticktask.DueDate,
            Priority = tickticktask.Priority,
            Completed = tickticktask.Completed,
            CreatedAt = tickticktask.CreatedAt,
            CompletedAt = tickticktask.CompletedAt,
            TaskTags = tickticktask.TaskTags.Select(t => t.ToDto()).ToList()
        };
        return dto;
    }

    public static TaskTagDto ToDto(this TaskTag taskTag)
    {
        var dto = new TaskTagDto
        {
            TaskId = taskTag.TaskId,
            TagId = taskTag.TagId,
            CreatedAt = taskTag.CreatedAt
        };
        return dto;
    }

    public static TagDto ToDto(this Tag tag)
    {
        var dto = new TagDto
        {
            TagId = tag.TagId,
            Name = tag.Name,
            UserId = tag.UserId,
            CreatedAt = tag.CreatedAt
        };
        return dto;
    }
}