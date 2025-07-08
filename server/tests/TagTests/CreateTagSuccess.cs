using System.ComponentModel.DataAnnotations;
using api.Models.Dtos.Requests;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.Tag;

public class CreateTagSuccess : ApiTestBase
{

    [Test]
    public async Task CreateTag()
    {
        var ctx = ScopedServiceProvider.GetRequiredService<MyDbContext>();
        var timeProvider = ScopedServiceProvider.GetRequiredService<TimeProvider>();
        
        var newTagName = "My new tag";
        var tag = new CreateTagRequestDto(newTagName);

        var result = (await ApiClient.TicktickTask_CreateTagAsync(tag)).Result;
            
        Validator.ValidateObject(result, new ValidationContext(result), true);
        
        if (result.Name != newTagName)
            throw new Exception($"Expected tag name to be '{newTagName}' but got '{result.Name}'");
            
        if (string.IsNullOrEmpty(result.TagId))
            throw new Exception("Created tag should have a non-empty TagId");
            
        // Verify tag exists in database
        var dbTag = ctx.Tags.FirstOrDefault(t => t.TagId == result.TagId);
        if (dbTag == null)
            throw new Exception($"Tag with ID {result.TagId} should exist in database after creation");
            
        if (dbTag.Name != newTagName)
            throw new Exception($"Database tag name should be '{newTagName}' but got '{dbTag.Name}'");
            
        // Verify CreatedAt is recent
        if (Math.Abs((timeProvider.GetUtcNow().UtcDateTime - result.CreatedAt).TotalSeconds) > 2)
            throw new Exception("CreatedAt timestamp should be within 2 seconds of now");
    }
}