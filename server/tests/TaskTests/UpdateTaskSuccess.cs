using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using api.Controllers;
using api.Etc;
using api.Mappers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.TaskTests;

public class UpdateTaskSuccess
{
    private WebApplication _app = null!;
    private string _baseUrl = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;

    [Before(Test)]
    public Task Setup()
    {
        var builder = ApiTestSetupUtilities.MakeWebAppBuilderForTesting();
        builder.AddProgramcsServices();
        builder.ModifyServicesForTesting();
        _app = builder.Build();

        _app.BeforeProgramcsMiddleware();
        _app.AddProgramcsMiddleware();
        _app.AfterProgramcsMiddleware();

        _baseUrl = _app.Urls.First() + "/";
        _scopedServiceProvider = _app.Services.CreateScope().ServiceProvider;
        _client = ApiTestSetupUtilities.CreateHttpClientWithDefaultTestJwt();
        return Task.CompletedTask;
    }

    [Test]
    public async Task UpdateTask_CanSuccessfullyUpdateTask()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        var timeProvider = _scopedServiceProvider.GetRequiredService<TimeProvider>();
        
        // Use existing CriticalBugTask from test data to update
        var taskToUpdateId = ids.CriticalBugTaskId;

        var request = new UpdateTaskRequestDto
        (
            taskToUpdateId,
            title: "Updated Title",
            description: "Updated Description",
            dueDate: timeProvider.GetUtcNow().AddDays(10).UtcDateTime.ToUniversalTime(),
            priority: 3,
            completed: true,
            listId: ids.PersonalListId // Moving from Work list to Personal list
        );
        
        var response = await _client.PatchAsJsonAsync(_baseUrl + nameof(TicktickTaskController.UpdateTask), request);
        
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Expected success status but got {response.StatusCode}. Response: {await response.Content.ReadAsStringAsync()}");
            
        var updatedTask = await response.Content.ReadFromJsonAsync<TickticktaskDto>();
        
        if (updatedTask == null)
            throw new Exception("Response body was null when deserializing to TickticktaskDto");

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