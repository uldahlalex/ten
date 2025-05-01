using efscaffold.Entities;

namespace api.Mappers;

public static class MapToDtosExtensions
{

    public static TasklistDto ToDto(this Tasklist entity)
    {
        var dto = new TasklistDto
        {
            ListId = entity.ListId,
            UserId = entity.UserId,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            Tickticktasks = entity.Tickticktasks.Select(t => t.ToDto()).ToList(),
            User = entity.User.ToDto()
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

    public static TickticktaskDto ToDto(this Tickticktask entity)
    {
        var dto = new TickticktaskDto
        {
            TaskId = entity.TaskId,
            ListId = entity.ListId,
            Title = entity.Title,
            Description = entity.Description,
            DueDate = entity.DueDate,
            Priority = entity.Priority,
            Completed = entity.Completed,
            CreatedAt = entity.CreatedAt,
            CompletedAt = entity.CompletedAt,
            TaskTags = entity.TaskTags.Select(t => t.ToDto()).ToList()
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