using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

/// <summary>
///     List is always created for the user sending the request
/// </summary>
public record CreateListRequestDto(
    [MinLength(1), Required] string ListName
);