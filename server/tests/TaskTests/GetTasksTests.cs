using System.ComponentModel.DataAnnotations;
using api.Etc;
using api.Models.Dtos.Requests;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.TaskTests;

public class GetTasksTests : ApiTestBase
{


    [Test]
    public Task GetTasks_ShouldReturnAllMyTasks_WhenNoFiltersApplied()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // John should have these specific tasks based on TestDataSeeder
        var expectedJohnTaskIds = new HashSet<string>
        {
            ids.CriticalBugTaskId,    // Work list task
            ids.SearchFeatureTaskId, // Work list task  
            ids.UpdateDocsTaskId,    // Work list task
            ids.GroceriesTaskId      // Personal list task
        };

        var query = new MyFilterss();

        var actualTasks = ApiClient
            .TicktickTask_GetMyTasksAsync(query)
            .GetAwaiter()
            .GetResult();

        if (actualTasks == null)
            throw new Exception("Response body was null when deserializing to List<TickticktaskDto>");
            
        if (actualTasks.Count != expectedJohnTaskIds.Count)
            throw new Exception($"Expected {expectedJohnTaskIds.Count} tasks for John but got {actualTasks.Count} tasks");
            
        var actualTaskIds = actualTasks.Select(t => t.TaskId).ToHashSet();
        
        if (!expectedJohnTaskIds.SetEquals(actualTaskIds))
            throw new Exception($"Task IDs don't match. Expected: [{string.Join(", ", expectedJohnTaskIds)}], Actual: [{string.Join(", ", actualTaskIds)}]");
        Validator.ValidateObject(actualTasks.First(), new ValidationContext(actualTasks.First()));

        return Task.CompletedTask;
    }


    [Test]
    public async Task GetTasks_ShouldFilterByCompletion()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Based on TestDataSeeder, only UpdateDocsTask is completed for John
        var expectedCompletedTaskIds = new HashSet<string>
        {
            ids.UpdateDocsTaskId // This is the only completed task in test data
        };

        var query = new MyFilterss { IsCompleted = true };

        var actualTasks = await ApiClient.TicktickTask_GetMyTasksAsync(query);
        
        if (actualTasks == null)
            throw new Exception("Response body was null when deserializing to List<TickticktaskDto>");
            
        if (actualTasks.Count != expectedCompletedTaskIds.Count)
            throw new Exception($"Expected {expectedCompletedTaskIds.Count} completed task but got {actualTasks.Count}");

        if (actualTasks.Any(t => t.Completed == false))
            throw new Exception("All returned tasks should be completed when filtering by IsCompleted=true");
            
        var actualTaskIds = actualTasks.Select(t => t.TaskId).ToHashSet();
        
        if (!expectedCompletedTaskIds.SetEquals(actualTaskIds))
            throw new Exception($"Expected completed task IDs: [{string.Join(", ", expectedCompletedTaskIds)}], but got: [{string.Join(", ", actualTaskIds)}]");
    }

    [Test]
    public async Task GetTasks_ShouldFilterByDateRange()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        var timeProvider = ScopedServiceProvider.GetRequiredService<TimeProvider>();
        
        var earliestDate = timeProvider.GetUtcNow().AddDays(-7).UtcDateTime;
        var latestDate = timeProvider.GetUtcNow().AddDays(6).UtcDateTime;
        
        // Based on TestDataSeeder, John's tasks have these due dates:
        // - CriticalBugTask: _baseTime.AddDays(1) -> should be in range
        // - SearchFeatureTask: _baseTime.AddDays(7) -> should NOT be in range  
        // - UpdateDocsTask: _baseTime.AddDays(3) -> should be in range
        // - GroceriesTask: _baseTime.AddDays(2) -> should be in range
        // All tasks should be within the -7 to +8 day range
        var expectedTaskIdsInRange = new HashSet<string>
        {
            ids.CriticalBugTaskId,
            ids.UpdateDocsTaskId,
            ids.GroceriesTaskId
        };

        var query = new MyFilterss
        {
            EarliestDueDate = earliestDate,
            LatestDueDate = latestDate
        };

        var actualTasks = await ApiClient.TicktickTask_GetMyTasksAsync(query);
        
        if (actualTasks == null)
            throw new Exception("Response body was null when deserializing to List<TickticktaskDto>");
            
        if (actualTasks.Any(t => t.DueDate > query.LatestDueDate || t.DueDate < query.EarliestDueDate))
            throw new Exception($"All tasks should be within date range {earliestDate:yyyy-MM-dd} to {latestDate:yyyy-MM-dd}");
            
        if (actualTasks.Count != expectedTaskIdsInRange.Count)
            throw new Exception($"Expected {expectedTaskIdsInRange.Count} tasks within date range but got {actualTasks.Count}");
            
        var actualTaskIds = actualTasks.Select(t => t.TaskId).ToHashSet();
        
        if (!expectedTaskIdsInRange.SetEquals(actualTaskIds))
            throw new Exception($"Expected task IDs in range: [{string.Join(", ", expectedTaskIdsInRange)}], but got: [{string.Join(", ", actualTaskIds)}]");
    }

    [Test]
    public async Task GetTasks_ShouldFilterByPriorityRange()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        var minPriority = 2;
        var maxPriority = 3;
        
        // Based on TestDataSeeder, John's tasks have these priorities:
        // - CriticalBugTask: priority 5 -> outside range
        // - SearchFeatureTask: priority 3 -> in range
        // - UpdateDocsTask: priority 2 -> in range  
        // - GroceriesTask: priority 2 -> in range
        var expectedTaskIdsInPriorityRange = new HashSet<string>
        {
            ids.SearchFeatureTaskId, // priority 3
            ids.UpdateDocsTaskId,    // priority 2
            ids.GroceriesTaskId      // priority 2
        };

        var query = new MyFilterss
        {
            MinPriority = minPriority,
            MaxPriority = maxPriority
        };

        var actualTasks = await ApiClient.TicktickTask_GetMyTasksAsync(query);
        
        if (actualTasks == null)
            throw new Exception("Response body was null when deserializing to List<TickticktaskDto>");
            
        if (actualTasks.Any(t => t.Priority < minPriority))
            throw new Exception($"Found tasks with priority below minimum {minPriority}");
            
        if (actualTasks.Any(t => t.Priority > maxPriority))
            throw new Exception($"Found tasks with priority above maximum {maxPriority}");
            
        if (actualTasks.Count != expectedTaskIdsInPriorityRange.Count)
            throw new Exception($"Expected {expectedTaskIdsInPriorityRange.Count} tasks with priority {minPriority}-{maxPriority} but got {actualTasks.Count}");
            
        var actualTaskIds = actualTasks.Select(t => t.TaskId).ToHashSet();
        
        if (!expectedTaskIdsInPriorityRange.SetEquals(actualTaskIds))
            throw new Exception($"Expected task IDs in priority range: [{string.Join(", ", expectedTaskIdsInPriorityRange)}], but got: [{string.Join(", ", actualTaskIds)}]");
    }
    
   
}