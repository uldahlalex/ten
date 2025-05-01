using System.Net;
using System.Net.Http.Json;
using api;
using api.Mappers;
using api.Seeder;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using tests;
using JsonSerializer = System.Text.Json.JsonSerializer;

[TestFixture]
public class GetTasksTests
{
    private WebApplication _app = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;
    private string _baseUrl = null!;

    [OneTimeSetUp]
    public async Task Setup()
    {
        var builder = WebApplication.CreateBuilder();
        
        Program.ConfigureServices(builder);
        var descriptor = builder.Services.FirstOrDefault(t => t.ServiceType == typeof(ISeeder));
        if(descriptor!=null)
            builder.Services.Remove(descriptor);
        builder.Services.AddScoped<ISeeder, DefaultEnvironment>();
        
        _app = builder.Build();
        Program.ConfigureApp(_app);
        
        await _app.StartAsync();
        _baseUrl = _app.Urls.First() + "/";
        Console.WriteLine($"Test API running at: {_baseUrl}");
        _scopedServiceProvider = _app.Services.CreateScope().ServiceProvider;

        
        _client = new HttpClient();
        await _client.TestRegisterAndAddJwt(_baseUrl);

    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        _client.Dispose();
            await _app.StopAsync();
            await _app.DisposeAsync();
        
    }



    [Test]
    public async Task GetTasks_ShouldReturnAllTasks_WhenNoFiltersApplied()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var allTasksAsDtos = ctx.Tickticktasks.Select(d => d.ToDto()).ToList();
        var query = new GetTasksFilterAndOrderParameters();
        

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks), query);
        
        if(response.StatusCode != HttpStatusCode.OK)
            throw new Exception("Did not get success status code. Received: "+response.StatusCode+ " with body: "+await response.Content.ReadAsStringAsync());
        var tasksResponse = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        if(tasksResponse!.Count != allTasksAsDtos.Count)
            throw new Exception("Did not get all tasks. Expected: "+JsonSerializer.Serialize(allTasksAsDtos) + " but got: "+JsonSerializer.Serialize(tasksResponse));
        
    }
    

    
    [Test]
    public async Task GetTasks_ShouldFilterByCompletion()
    {
        // Arrange
        var query = new GetTasksFilterAndOrderParameters() { IsCompleted = true };
        
        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks), query);
     
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        if(tasks.Count==0)
            throw new Exception("There should be at least one task");
        
        if(tasks.Any(t => t.Completed == false))
            throw new Exception("There shoud not be any incompleted tasks");
    }
    
    [Test]
    public async Task GetTasks_ShouldFilterByDateRange()
    {
        // Arrange
        var query = new GetTasksFilterAndOrderParameters()
        {
            EarliestDueDate = DateTime.UtcNow.AddDays(-7),
            LatestDueDate = DateTime.UtcNow.AddDays(7)
        };
    
    
        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks),query);
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
          if(tasks.Any(t => t.DueDate > query.LatestDueDate || t.DueDate < query.EarliestDueDate))
                throw new Exception("There should not be any tasks outside of the date range");
        if (tasks.Count != 80)
            throw new Exception("Expected exactly 80 tasks from the default seeder");
        
        
    }
    
    [Test]
    public async Task GetTasks_ShouldFilterByPriorityRange()
    {
        // Arrange
        var query = new GetTasksFilterAndOrderParameters()
        {
            MinPriority = 2,
            MaxPriority = 3
        };
        await _client.TestRegisterAndAddJwt(_baseUrl);
    
    
        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.GetMyTasks),query);
        var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
        if (tasks.Any(t => t.Priority < query.MinPriority))
            throw new Exception("There were priorities under the threshold");
        if(tasks.Any(t => t.Priority > query.MaxPriority))
            throw new Exception("There were priorities above the threshold");
        if (tasks.Count != 10)
            throw new Exception("Expected exactly 10 from the deafult seeder");
    }
    //
    // [Test]
    // public async Task GetTasks_ShouldFilterBySearchTerm()
    // {
    //     // Arrange
    //     var query = new TaskQueryParams { SearchTerm = "project" };
    //     await _httpClient.TestRegisterAndAddJwt();
    //
    //     // Act
    //     var response = await _httpClient.GetAsync(
    //         TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
    //     
    //     // Assert
    //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    //     var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
    //     Assert.That(tasks, Is.Not.Null);
    //     Assert.That(tasks!.All(t => 
    //         t.Title.ToLower().Contains(query.SearchTerm!.ToLower()) || 
    //         t.Description.ToLower().Contains(query.SearchTerm.ToLower())), Is.True);
    // }
    //
    // [Test]
    // public async Task GetTasks_ShouldFilterByTags()
    // {
    //     // Arrange
    //     var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
    //     var tagId = ctx.Tags.First().TagId;
    //     var query = new TaskQueryParams { TagIds = new List<string> { tagId } };
    //     await _httpClient.TestRegisterAndAddJwt();
    //
    //
    //     // Act
    //     var response = await _httpClient.GetAsync(
    //         "/GetMyTasks");
    //     
    //     // Assert
    //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    //     List<TickticktaskDto> tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>() ?? throw new Exception("Failed to deserialize tasks");
    //     var tasksAsJsonText = JsonSerializer.Serialize(tasks);
    //     Console.WriteLine(tasksAsJsonText);
    //     Assert.That(tasks, Is.Not.Null);
    //     Assert.That(tasks!.Count, Is.GreaterThan(0));
    // }
    //
    // [Test]
    // public async Task GetTasks_ShouldFilterByLists()
    // {
    //     // Arrange
    //     var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
    //     var listId = ctx.Tasklists.First().ListId;
    //     var query = new TaskQueryParams { ListIds = new List<string> { listId } };
    //     await _httpClient.TestRegisterAndAddJwt();
    //
    //
    //     // Act
    //     var response = await _httpClient.GetAsync(
    //         TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
    //     
    //     // Assert
    //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    //     var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
    //     Assert.That(tasks, Is.Not.Null);
    //     Assert.That(tasks!.All(t => t.ListId == listId), Is.True);
    // }
    //
    // [Test]
    // [TestCase(nameof(Tickticktask.DueDate), true)]
    // [TestCase(nameof(Tickticktask.Priority), false)]
    // [TestCase(nameof(Tickticktask.CreatedAt), true)]
    // [TestCase(nameof(Tickticktask.CompletedAt), false)]
    // public async Task GetTasks_ShouldOrderCorrectly(string orderByValue, bool isDescending)
    // {
    //     // Arrange
    //     TaskOrderBy? orderBy;
    //     TaskOrderBy.TryParse(orderByValue, out orderBy);
    //
    //     var query = new TaskQueryParams
    //     {
    //         OrderBy = orderBy,
    //         IsDescending = isDescending
    //     };
    //
    //     await _httpClient.TestRegisterAndAddJwt();
    //
    //
    //     // Act
    //     var response = await _httpClient.GetAsync(
    //         TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
    //
    //     // Assert
    //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    //     var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
    //     Assert.That(tasks, Is.Not.Null);
    //
    //
    //     IEnumerable<TickticktaskDto> orderedTasks;
    //     if (orderBy?.Value is var x1 && x1 == TaskOrderBy.DueDate.Value)
    //         orderedTasks = isDescending ? tasks!.OrderByDescending(t => t.DueDate) : tasks!.OrderBy(t => t.DueDate);
    //     else if (orderBy?.Value is var x2 && x2 == TaskOrderBy.Priority.Value)
    //         orderedTasks = isDescending
    //             ? tasks!.OrderByDescending(t => t.Priority)
    //             : tasks!.OrderBy(t => t.Priority);
    //     else if (orderBy?.Value is var x3 && x3 == TaskOrderBy.CreatedAt.Value)
    //         orderedTasks = isDescending
    //             ? tasks!.OrderByDescending(t => t.CreatedAt)
    //             : tasks!.OrderBy(t => t.CreatedAt);
    //     else if (orderBy?.Value is var x4 && x4 == TaskOrderBy.CompletedAt.Value)
    //         orderedTasks = isDescending
    //             ? tasks!.OrderByDescending(t => t.CompletedAt)
    //             : tasks!.OrderBy(t => t.CompletedAt);
    //     else
    //         orderedTasks = tasks!;
    //
    //     Assert.That(tasks, Is.EqualTo(orderedTasks.ToList()));
    //
    // }
    //
    // [Test]
    // public async Task GetTasks_ShouldCombineMultipleFilters()
    // {
    //     // Arrange
    //     var query = new TaskQueryParams
    //     {
    //         IsCompleted = false,
    //         DueDateStart = DateTime.UtcNow,
    //         DueDateEnd = DateTime.UtcNow.AddDays(30),
    //         MinPriority = 2,
    //         OrderBy = TaskOrderBy.Priority,
    //         IsDescending = true
    //     };
    //     await _httpClient.TestRegisterAndAddJwt();
    //
    //     // Act
    //     var response = await _httpClient.GetAsync(
    //         TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
    //     
    //     // Assert
    //     Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    //     var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
    //     Assert.That(tasks, Is.Not.Null);
    //     
    //     Assert.That(tasks!.All(t => 
    //         !t.Completed &&
    //         t.DueDate >= query.DueDateStart &&
    //         t.DueDate <= query.DueDateEnd &&
    //         t.Priority >= query.MinPriority), Is.True);
    //     
    //     Assert.That(tasks, Is.EqualTo(tasks.OrderByDescending(t => t.Priority)));
    // }
}