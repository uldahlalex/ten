using System.ComponentModel.DataAnnotations;
using api.Etc;
using api.Models.Dtos.Requests;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.TaskTests;

public class CreateTaskTestsSuccess : ApiTestBase
{


    [Test]
    public async Task CreateTask_ShouldReturnOk_WhenValidRequest()
    {
        var ctx = ScopedServiceProvider.GetRequiredService<MyDbContext>();
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        var timeProvider = ScopedServiceProvider.GetRequiredService<TimeProvider>();
        
        var dueDate = timeProvider.GetUtcNow().AddDays(1).UtcDateTime;

        // Use John's Work list from test data
        var request = new CreateTaskRequestDto
        (
            ids.WorkListId,
            "Test Task",
            "Test Description",
            dueDate,
            1
        );

        var responseBodyAsDto = await ApiClient.TicktickTask_CreateTaskAsync(request);
            
        // Validate the response DTO
        Validator.ValidateObject(responseBodyAsDto, new ValidationContext(responseBodyAsDto), true);
        
        if (responseBodyAsDto.Title != "Test Task")
            throw new Exception($"Expected task title to be 'Test Task' but got '{responseBodyAsDto.Title}'");
            
        if (responseBodyAsDto.Description != "Test Description")
            throw new Exception($"Expected task description to be 'Test Description' but got '{responseBodyAsDto.Description}'");
            
        if (responseBodyAsDto.ListId != ids.WorkListId)
            throw new Exception($"Expected task to be in Work list {ids.WorkListId} but got {responseBodyAsDto.ListId}");
            
        if (responseBodyAsDto.Priority != 1)
            throw new Exception($"Expected task priority to be 1 but got {responseBodyAsDto.Priority}");
            
        // Verify task exists in database
        var dbTask = ctx.Tickticktasks.FirstOrDefault(t => t.TaskId == responseBodyAsDto.TaskId);
        if (dbTask == null)
            throw new Exception($"Task with ID {responseBodyAsDto.TaskId} should exist in database after creation");
            
        Validator.ValidateObject(dbTask, new ValidationContext(dbTask), true);
        
        // Verify CreatedAt is recent
        if (Math.Abs((timeProvider.GetUtcNow().UtcDateTime - responseBodyAsDto.CreatedAt).TotalSeconds) > 2)
            throw new Exception("CreatedAt timestamp should be within 2 seconds of now");
    }
}