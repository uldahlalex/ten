// ReSharper disable once CheckNamespace
namespace efscaffold.Entities;

public partial class Tag
{
    /// <summary>
    /// Default private parameterless ctor to be used by EF Core.
    /// </summary>
    private Tag()
    {
        
    }
    public Tag(DateTime createdAt, string name, string userId, string? id = null)
    {
        TagId = id ?? Guid.NewGuid().ToString();
        Name = name;
        UserId = userId;
        CreatedAt = createdAt;

    }
}