namespace api.Models.Dtos.Responses;

public record TasklistDto(
    string ListId,
    string UserId,
    string Name,
    DateTime CreatedAt
);