using System.Net;
using System.Net.Http.Json;
using api;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using tests;
using JsonSerializer = System.Text.Json.JsonSerializer;

[TestFixture]
public class GetTasksTests
{
    private HttpClient _httpClient;
    private IServiceProvider _scopedServiceProvider;

    [SetUp]
    public async Task Setup()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.DefaultTestConfig(useTestContainer: false);
                });
            });

        _httpClient = factory.CreateClient();
        _scopedServiceProvider = factory.Services.CreateScope().ServiceProvider;
        await _httpClient.TestRegisterAndAddJwt();
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    [Test]
    public async Task GetTasks_ShouldReturnAllTasks_WhenNoFiltersApplied()
    {
        // Arrange
        var query = new TaskQueryParams();

        // Act
        var response = await _httpClient.GetAsync(
            "GetMyTasks");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        Console.WriteLine(JsonSerializer.Serialize(tasks));
        Assert.That(tasks, Is.Not.Null);
        Assert.That(tasks!.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetTasks_ShouldFilterByCompletion()
    {
        // Arrange
        var query = new TaskQueryParams { IsCompleted = true };

        // Act
        var response = await _httpClient.GetAsync(
            TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        Assert.That(tasks, Is.Not.Null);
        Assert.That(tasks!.All(t => t.Completed), Is.True);
    }

    [Test]
    public async Task GetTasks_ShouldFilterByDateRange()
    {
        // Arrange
        var query = new TaskQueryParams
        {
            DueDateStart = DateTime.UtcNow.AddDays(-7),
            DueDateEnd = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var response = await _httpClient.GetAsync(
            TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        Assert.That(tasks, Is.Not.Null);
        Assert.That(tasks!.All(t => 
            t.DueDate >= query.DueDateStart && 
            t.DueDate <= query.DueDateEnd), Is.True);
    }

    [Test]
    public async Task GetTasks_ShouldFilterByPriorityRange()
    {
        // Arrange
        var query = new TaskQueryParams
        {
            MinPriority = 2,
            MaxPriority = 3
        };

        // Act
        var response = await _httpClient.GetAsync(
            TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        Assert.That(tasks, Is.Not.Null);
        Assert.That(tasks!.All(t => t.Priority >= query.MinPriority && t.Priority <= query.MaxPriority), Is.True);
    }

    [Test]
    public async Task GetTasks_ShouldFilterBySearchTerm()
    {
        // Arrange
        var query = new TaskQueryParams { SearchTerm = "project" };

        // Act
        var response = await _httpClient.GetAsync(
            TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        Assert.That(tasks, Is.Not.Null);
        Assert.That(tasks!.All(t => 
            t.Title.ToLower().Contains(query.SearchTerm!.ToLower()) || 
            t.Description.ToLower().Contains(query.SearchTerm.ToLower())), Is.True);
    }

    [Test]
    public async Task GetTasks_ShouldFilterByTags()
    {
        // Arrange
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var tagId = ctx.Tags.First().TagId;
        var query = new TaskQueryParams { TagIds = new List<string> { tagId } };

        // Act
        var response = await _httpClient.GetAsync(
            "/GetMyTasks");
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        List<TickticktaskDto> tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>() ?? throw new Exception("Failed to deserialize tasks");
        var tasksAsJsonText = System.Text.Json.JsonSerializer.Serialize(tasks);
        Console.WriteLine(tasksAsJsonText);
        Assert.That(tasks, Is.Not.Null);
        Assert.That(tasks!.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetTasks_ShouldFilterByLists()
    {
        // Arrange
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var listId = ctx.Tasklists.First().ListId;
        var query = new TaskQueryParams { ListIds = new List<string> { listId } };

        // Act
        var response = await _httpClient.GetAsync(
            TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        Assert.That(tasks, Is.Not.Null);
        Assert.That(tasks!.All(t => t.ListId == listId), Is.True);
    }

    [Test]
    [TestCase(nameof(Tickticktask.DueDate), true)]
    [TestCase(nameof(Tickticktask.Priority), false)]
    [TestCase(nameof(Tickticktask.CreatedAt), true)]
    [TestCase(nameof(Tickticktask.CompletedAt), false)]
    public async Task GetTasks_ShouldOrderCorrectly(string orderByValue, bool isDescending)
    {
        // Arrange
        TaskOrderBy? orderBy;
        TaskOrderBy.TryParse(orderByValue, out orderBy);

        var query = new TaskQueryParams
        {
            OrderBy = orderBy,
            IsDescending = isDescending
        };


        // Act
        var response = await _httpClient.GetAsync(
            TicktickTaskController.GetMyTasksRoute + query.ToQueryString());

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        Assert.That(tasks, Is.Not.Null);


        IEnumerable<TickticktaskDto> orderedTasks;
        if (orderBy?.Value is var x1 && x1 == TaskOrderBy.DueDate.Value)
            orderedTasks = isDescending ? tasks!.OrderByDescending(t => t.DueDate) : tasks!.OrderBy(t => t.DueDate);
        else if (orderBy?.Value is var x2 && x2 == TaskOrderBy.Priority.Value)
            orderedTasks = isDescending
                ? tasks!.OrderByDescending(t => t.Priority)
                : tasks!.OrderBy(t => t.Priority);
        else if (orderBy?.Value is var x3 && x3 == TaskOrderBy.CreatedAt.Value)
            orderedTasks = isDescending
                ? tasks!.OrderByDescending(t => t.CreatedAt)
                : tasks!.OrderBy(t => t.CreatedAt);
        else if (orderBy?.Value is var x4 && x4 == TaskOrderBy.CompletedAt.Value)
            orderedTasks = isDescending
                ? tasks!.OrderByDescending(t => t.CompletedAt)
                : tasks!.OrderBy(t => t.CompletedAt);
        else
            orderedTasks = tasks!;

        Assert.That(tasks, Is.EqualTo(orderedTasks.ToList()));

    }

    [Test]
    public async Task GetTasks_ShouldCombineMultipleFilters()
    {
        // Arrange
        var query = new TaskQueryParams
        {
            IsCompleted = false,
            DueDateStart = DateTime.UtcNow,
            DueDateEnd = DateTime.UtcNow.AddDays(30),
            MinPriority = 2,
            OrderBy = TaskOrderBy.Priority,
            IsDescending = true
        };

        // Act
        var response = await _httpClient.GetAsync(
            TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        Assert.That(tasks, Is.Not.Null);
        
        Assert.That(tasks!.All(t => 
            !t.Completed &&
            t.DueDate >= query.DueDateStart &&
            t.DueDate <= query.DueDateEnd &&
            t.Priority >= query.MinPriority), Is.True);
        
        Assert.That(tasks, Is.EqualTo(tasks.OrderByDescending(t => t.Priority)));
    }
}