using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Etc;
using api.Mappers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace tests.TaskTests;

public class GetTasksTests
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
    public Task GetTasks_ShouldReturnAllMyTasks_WhenNoFiltersApplied()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Get expected tasks from DB using known IDs
        var expectedTasks = ctx.Tickticktasks
            .Include(t => t.List)
            .Where(t => t.List.UserId == ids.JohnId)
            .ToList();

        var query = new GetTasksFilterAndOrderParameters() { };

        var response = _client
            .PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks), query)
            .GetAwaiter()
            .GetResult();

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Expected OK status but got {response.StatusCode}. Response: {response.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
            
        var actualTasks = response.Content
            .ReadFromJsonAsync<List<TickticktaskDto>>()
            .GetAwaiter()
            .GetResult();

        if (actualTasks == null)
            throw new Exception("Response body was null when deserializing to List<TickticktaskDto>");
            
        if (expectedTasks.Count != actualTasks.Count)
            throw new Exception($"Expected {expectedTasks.Count} tasks for John but got {actualTasks.Count} tasks");
            
        // Verify specific expected tasks are present
        var expectedTaskIds = expectedTasks.Select(t => t.TaskId).ToHashSet();
        var actualTaskIds = actualTasks.Select(t => t.TaskId).ToHashSet();
        
        if (!expectedTaskIds.SetEquals(actualTaskIds))
            throw new Exception($"Task IDs don't match. Expected: [{string.Join(", ", expectedTaskIds)}], Actual: [{string.Join(", ", actualTaskIds)}]");
            
        return Task.CompletedTask;
    }


    [Test]
    public async Task GetTasks_ShouldFilterByCompletion()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Get expected completed tasks from DB
        var expectedCompletedTasks = ctx.Tickticktasks
            .Include(t => t.List)
            .Where(t => t.List.UserId == ids.JohnId && t.Completed == true)
            .ToList();

        var query = new GetTasksFilterAndOrderParameters { IsCompleted = true};

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks), query);

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Expected OK status but got {response.StatusCode}. Response: {await response.Content.ReadAsStringAsync()}");

        var actualTasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        
        if (actualTasks == null)
            throw new Exception("Response body was null when deserializing to List<TickticktaskDto>");

        if (expectedCompletedTasks.Count == 0)
            throw new Exception("Test data should contain at least one completed task for John");
            
        if (actualTasks.Count != expectedCompletedTasks.Count)
            throw new Exception($"Expected {expectedCompletedTasks.Count} completed tasks but got {actualTasks.Count}");

        if (actualTasks.Any(t => t.Completed == false))
            throw new Exception("All returned tasks should be completed when filtering by IsCompleted=true");
            
        // Verify we got the specific completed task from test data
        var expectedTaskId = ids.UpdateDocsTaskId; // This is the completed task in test data
        if (!actualTasks.Any(t => t.TaskId == expectedTaskId))
            throw new Exception($"Expected to find completed task {expectedTaskId} in results");
    }

    [Test]
    public async Task GetTasks_ShouldFilterByDateRange()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        var timeProvider = _scopedServiceProvider.GetRequiredService<TimeProvider>();
        
        var earliestDate = timeProvider.GetUtcNow().AddDays(-7).UtcDateTime;
        var latestDate = timeProvider.GetUtcNow().AddDays(8).UtcDateTime;
        
        // Get expected tasks within date range from DB
        var expectedTasks = ctx.Tickticktasks
            .Include(t => t.List)
            .Where(t => t.List.UserId == ids.JohnId && 
                       t.DueDate >= earliestDate && 
                       t.DueDate <= latestDate)
            .ToList();

        var query = new GetTasksFilterAndOrderParameters
        {
            EarliestDueDate = earliestDate,
            LatestDueDate = latestDate
        };

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks), query);
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Expected OK status but got {response.StatusCode}. Response: {await response.Content.ReadAsStringAsync()}");
            
        var actualTasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        
        if (actualTasks == null)
            throw new Exception("Response body was null when deserializing to List<TickticktaskDto>");
            
        if (actualTasks.Any(t => t.DueDate > query.LatestDueDate || t.DueDate < query.EarliestDueDate))
            throw new Exception($"All tasks should be within date range {earliestDate:yyyy-MM-dd} to {latestDate:yyyy-MM-dd}");
            
        if (actualTasks.Count != expectedTasks.Count)
            throw new Exception($"Expected {expectedTasks.Count} tasks within date range but got {actualTasks.Count}");
            
        // Verify specific tasks that should be in this range based on test data
        var expectedTaskIds = expectedTasks.Select(t => t.TaskId).ToHashSet();
        var actualTaskIds = actualTasks.Select(t => t.TaskId).ToHashSet();
        
        if (!expectedTaskIds.SetEquals(actualTaskIds))
            throw new Exception($"Task IDs don't match expected. Expected: [{string.Join(", ", expectedTaskIds)}], Actual: [{string.Join(", ", actualTaskIds)}]");
    }

    [Test]
    public async Task GetTasks_ShouldFilterByPriorityRange()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        var minPriority = 2;
        var maxPriority = 3;
        
        // Get expected tasks within priority range from DB
        var expectedTasks = ctx.Tickticktasks
            .Include(t => t.List)
            .Where(t => t.List.UserId == ids.JohnId && 
                       t.Priority >= minPriority && 
                       t.Priority <= maxPriority)
            .ToList();

        var query = new GetTasksFilterAndOrderParameters
        {
            MinPriority = minPriority,
            MaxPriority = maxPriority
        };

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks), query);
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Expected OK status but got {response.StatusCode}. Response: {await response.Content.ReadAsStringAsync()}");
            
        var actualTasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        
        if (actualTasks == null)
            throw new Exception("Response body was null when deserializing to List<TickticktaskDto>");
            
        if (actualTasks.Any(t => t.Priority < minPriority))
            throw new Exception($"Found tasks with priority below minimum {minPriority}");
            
        if (actualTasks.Any(t => t.Priority > maxPriority))
            throw new Exception($"Found tasks with priority above maximum {maxPriority}");
            
        if (actualTasks.Count != expectedTasks.Count)
            throw new Exception($"Expected {expectedTasks.Count} tasks with priority {minPriority}-{maxPriority} but got {actualTasks.Count}");
            
        // Verify specific tasks that should be in this priority range
        var expectedTaskIds = expectedTasks.Select(t => t.TaskId).ToHashSet();
        var actualTaskIds = actualTasks.Select(t => t.TaskId).ToHashSet();
        
        if (!expectedTaskIds.SetEquals(actualTaskIds))
            throw new Exception($"Task IDs don't match expected. Expected: [{string.Join(", ", expectedTaskIds)}], Actual: [{string.Join(", ", actualTaskIds)}]");

        // Based on test data, tasks with priority 2-3 should include UpdateDocsTask (priority 2) and SearchFeatureTask (priority 3)
        if (!actualTasks.Any(t => t.TaskId == ids.UpdateDocsTaskId))
            throw new Exception($"Expected to find UpdateDocsTask (priority 2) in results");
        if (!actualTasks.Any(t => t.TaskId == ids.SearchFeatureTaskId))
            throw new Exception($"Expected to find SearchFeatureTask (priority 3) in results");
    }
    
   
}