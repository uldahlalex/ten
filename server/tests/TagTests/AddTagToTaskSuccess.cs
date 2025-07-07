using System.ComponentModel.DataAnnotations;
using api.Etc;
using api.Models.Dtos.Requests;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.Tag;

public class AddTagToTaskSuccess : ApiTestBase
{


    [Test]
    public async Task AddTagToTask_CanSuccessfully_AddTagToTask()
    {
        var ctx = ScopedServiceProvider.GetRequiredService<MyDbContext>();
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Based on TestDataSeeder, we know:
        // - FeatureTag is NOT assigned to GroceriesTask
        // - So we can add FeatureTag to GroceriesTask
        var tagId = ids.FeatureTagId;
        var taskId = ids.GroceriesTaskId;
        
        // Verify this tag-task combination doesn't exist yet
        var existingTaskTag = ctx.TaskTags.FirstOrDefault(tt => tt.TagId == tagId && tt.TaskId == taskId);
        if (existingTaskTag != null)
            throw new Exception($"TaskTag relationship between {tagId} and {taskId} should not exist yet in test data");

        var dto = new ChangeTaskTagRequestDto(tagId, taskId);

        var result = await ApiClient.TicktickTask_AddTaskTagAsync(dto);
            
        Validator.ValidateObject(result, new ValidationContext(result), true);
        
        if (result.TagId != tagId)
            throw new Exception($"Expected TagId to be {tagId} but got {result.TagId}");
            
        if (result.TaskId != taskId)
            throw new Exception($"Expected TaskId to be {taskId} but got {result.TaskId}");
        
        // Verify the relationship now exists in database
        var createdTaskTag = ctx.TaskTags.FirstOrDefault(t => t.TagId == tagId && t.TaskId == taskId);
        if (createdTaskTag == null)
            throw new Exception($"TaskTag relationship between {tagId} and {taskId} should exist in database after creation");
    }
}