using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Etc;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Generated;

namespace tests.TaskTests;

public class CreateTaskTestsSuccess
{
    private WebApplication _app = null!;
    private string _baseUrl = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;
    private IApiClient _apiClient = null!;

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
        _apiClient = new ApiClient(_baseUrl, _client);
        return Task.CompletedTask;
    }


    [Test]
    public async Task CreateTask_ShouldReturnOk_WhenValidRequest()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        var timeProvider = _scopedServiceProvider.GetRequiredService<TimeProvider>();
        
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

        var responseBodyAsDto = await _apiClient.TicktickTask_CreateTaskAsync(request);
            
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