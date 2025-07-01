using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Mappers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
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
    public Task GetTasks_ShouldReturnAllTasks_WhenNoFiltersApplied()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var count = ctx.Tickticktasks.Count();
        if (count == 0)
            throw new Exception("There are no tasks starting out");
        var allTasksAsDtos = ctx.Tickticktasks.Select(d => d.ToDto()).ToList();
        var query = new GetTasksFilterAndOrderParameters();


        var response = _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks), query).GetAwaiter()
            .GetResult();

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception("Did not get success status code. Received: " + response.StatusCode + " with body: " +
                                response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        var tasksResponse = response.Content.ReadFromJsonAsync<List<TickticktaskDto>>().GetAwaiter().GetResult();
        if (tasksResponse!.Count != allTasksAsDtos.Count)
            throw new Exception("Did not get all tasks. Expected: " + JsonSerializer.Serialize(allTasksAsDtos) +
                                " but got: " + JsonSerializer.Serialize(tasksResponse));
        return Task.CompletedTask;
    }


    [Test]
    public async Task GetTasks_ShouldFilterByCompletion()
    {
        // Arrange
        var query = new GetTasksFilterAndOrderParameters { IsCompleted = true };

        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks), query);

        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        if (tasks.Count == 0)
            throw new Exception("There should be at least one task");

        if (tasks.Any(t => t.Completed == false))
            throw new Exception("There shoud not be any incompleted tasks");
    }

    [Test]
    public async Task GetTasks_ShouldFilterByDateRange()
    {
        // Arrange
        var query = new GetTasksFilterAndOrderParameters
        {
            EarliestDueDate = _scopedServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().AddDays(-7).UtcDateTime,
            LatestDueDate = _scopedServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().AddDays(7).UtcDateTime
        };


        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks), query);
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        if (tasks.Any(t => t.DueDate > query.LatestDueDate || t.DueDate < query.EarliestDueDate))
            throw new Exception("There should not be any tasks outside of the date range");
        if (tasks.Count != 80)
            throw new Exception("Expected exactly 80 tasks from the default seeder");
    }

    [Test]
    public async Task GetTasks_ShouldFilterByPriorityRange()
    {
        // Arrange
        var query = new GetTasksFilterAndOrderParameters
        {
            MinPriority = 2,
            MaxPriority = 3
        };


        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks), query);
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        if (tasks.Any(t => t.Priority < query.MinPriority))
            throw new Exception("There were priorities under the threshold");
        if (tasks.Any(t => t.Priority > query.MaxPriority))
            throw new Exception("There were priorities above the threshold");
        if (tasks.Count != 60)
            throw new Exception("Expected exactly 10 from the deafult seeder, but received: " + tasks.Count);
    }
    
   
}