using System.ComponentModel.DataAnnotations;
using api.Etc;
using api.Mappers;
using api.Models.Dtos.Requests;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.TaskTests;

public class UpdateTaskSuccess : ApiTestBase
{

    [Test]
    public async Task UpdateTask_CanSuccessfullyUpdateTask()
    {
        var ctx = ScopedServiceProvider.GetRequiredService<MyDbContext>();
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        var timeProvider = ScopedServiceProvider.GetRequiredService<TimeProvider>();
        
        // Use existing CriticalBugTask from test data to update
        var taskToUpdateId = ids.CriticalBugTaskId;

        var request = new UpdateTaskRequestDto
        (
            taskToUpdateId,
            ids.PersonalListId, // Moving from Work list to Personal list
            true,
            "Updated Title",
            "Updated Description",
            timeProvider.GetUtcNow().AddDays(10).UtcDateTime.ToUniversalTime(),
            3
        );
        
        var updatedTask = await ApiClient.TicktickTask_UpdateTaskAsync(request);

        // Verify task was updated in database
        var taskInDb = ctx.Tickticktasks.FirstOrDefault(t => t.TaskId == taskToUpdateId);
        if (taskInDb == null)
            throw new Exception($"Task with ID {taskToUpdateId} should exist in database");
            
        var taskDto = taskInDb.ToDto();
        Validator.ValidateObject(taskDto, new ValidationContext(taskDto), true);
        
        if (updatedTask.Title != request.Title)
            throw new Exception($"Expected title to be '{request.Title}' but got '{updatedTask.Title}'");
            
        if (updatedTask.Description != request.Description)
            throw new Exception($"Expected description to be '{request.Description}' but got '{updatedTask.Description}'");
            
        if (updatedTask.DueDate != request.DueDate)
            throw new Exception($"Expected due date to be {request.DueDate} but got {updatedTask.DueDate}");
            
        if (updatedTask.Priority != request.Priority)
            throw new Exception($"Expected priority to be {request.Priority} but got {updatedTask.Priority}");
            
        if (updatedTask.Completed != request.Completed)
            throw new Exception($"Expected completed to be {request.Completed} but got {updatedTask.Completed}");
            
        if (updatedTask.ListId != request.ListId)
            throw new Exception($"Expected task to be moved to list {request.ListId} but got {updatedTask.ListId}");
            
        if (updatedTask.CompletedAt == null)
            throw new Exception("CompletedAt timestamp should be set when task is marked as completed");
            
        // Verify CompletedAt is recent since we just completed it
        if (Math.Abs((timeProvider.GetUtcNow().UtcDateTime - updatedTask.CompletedAt.Value).TotalSeconds) > 2)
            throw new Exception("CompletedAt timestamp should be within 2 seconds of now");
    }
}