namespace api.Models.Dtos.Responses;

public record TaskTagDto(
    string TaskId,
    string TagId,
    DateTime CreatedAt
);