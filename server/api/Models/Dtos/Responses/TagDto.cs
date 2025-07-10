namespace api.Models.Dtos.Responses;

public record TagDto(
    string TagId,
    string Name,
    string UserId,
    DateTime CreatedAt
);