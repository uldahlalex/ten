using api.Models.Dtos.Responses;
using efscaffold.Entities;

namespace api.Mappers;

public static class EntityToDtosExtensions
{
    public static TasklistDto ToDto(this Tasklist entity)
    {
        return new TasklistDto(
            entity.ListId,
            entity.UserId,
            entity.Name,
            entity.CreatedAt
        );
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.UserId,
            user.Email,
            user.Salt,
            user.PasswordHash,
            user.Role,
            user.CreatedAt,
            user.Tags.Select(t => t.ToDto()).ToList(),
            user.Tasklists.Select(t => t.ToDto()).ToList()
        );
    }

    public static TickticktaskDto ToDto(this Tickticktask entity)
    {
        return new TickticktaskDto(
            entity.TaskId,
            entity.ListId,
            entity.Title,
            entity.Description,
            entity.DueDate,
            entity.Priority,
            entity.Completed,
            entity.CreatedAt,
            entity.CompletedAt,
            entity.TaskTags.Select(t => t.ToDto()).ToList()
        );
    }

    public static TaskTagDto ToDto(this TaskTag taskTag)
    {
        return new TaskTagDto(
            taskTag.TaskId,
            taskTag.TagId,
            taskTag.CreatedAt
        );
    }

    public static TagDto ToDto(this Tag tag)
    {
        return new TagDto(
            tag.TagId,
            tag.Name,
            tag.UserId,
            tag.CreatedAt
        );
    }
}