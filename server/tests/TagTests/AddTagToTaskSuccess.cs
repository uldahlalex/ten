using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Etc;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace tests.Tag;

public class AddTagToTaskSuccess
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
    public async Task AddTagToTask_CanSuccessfully_AddTagToTask()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        
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

        var response = await _client.PutAsJsonAsync(_baseUrl + nameof(TicktickTaskController.AddTaskTag), dto);
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Expected OK status but got {response.StatusCode}. Response: {await response.Content.ReadAsStringAsync()}");

        var result = await response.Content.ReadFromJsonAsync<TaskTagDto>();
        
        if (result == null)
            throw new Exception("Response body was null when deserializing to TaskTagDto");
            
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