// using System.Net;
// using System.Net.Http.Json;
// using api;
// using api.Seeder;
// using efscaffold.Entities;
// using Infrastructure.Postgres.Scaffolding;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
// using NUnit.Framework;
// using tests;
// using JsonSerializer = System.Text.Json.JsonSerializer;
//
// [TestFixture]
// public class GetTasksTests
// {
//     [SetUp]
//     public async Task Setup()
//     {
//         var factory = new WebApplicationFactory<Program>()
//             .WithWebHostBuilder(builder =>
//             {
//                 builder.ConfigureServices(services => { services.DefaultTestConfig(
//                     useTestContainer:true); });
//             });
//
//         _httpClient = factory.CreateClient();
//         _scopedServiceProvider = factory.Services.CreateScope().ServiceProvider;
//                 var dbContext = _scopedServiceProvider.GetRequiredService<MyDbContext>();
//         var connection = dbContext.Database.GetConnectionString();
//         Console.WriteLine($"Database connection string: {connection}");
//
//         // Method 2: Get it from DbContextOptions
//         var dbOptions = _scopedServiceProvider.GetRequiredService<DbContextOptions<MyDbContext>>();
//         var extension = dbOptions.Extensions.OfType<NpgsqlOptionsExtension>().FirstOrDefault();
//         if (extension != null)
//         {
//             Console.WriteLine($"Connection string from options: {extension.ConnectionString}");
//         }
//         
//         // Ensure database is created and seeded
//         await dbContext.Database.EnsureCreatedAsync();
//         
//         // Run seeder explicitly
//         var seeder = _scopedServiceProvider.GetRequiredService<IDefaultSeeder>();
//         await seeder.CreateEnvironment(dbContext);
//         
//         // Verify data exists
//         var taskCount = await dbContext.Tickticktasks.CountAsync();
//         Console.WriteLine($"Number of tasks in database: {taskCount}");
//         
//     
//     }
//
//     [TearDown]
//     public void TearDown()
//     {
//         _httpClient?.Dispose();
//     }
//
//     private HttpClient _httpClient;
//     private IServiceProvider _scopedServiceProvider;
//
//
//     [Test]
//     public async Task GetTasks_ShouldReturnAllTasks_WhenNoFiltersApplied()
//     {
//         // Log current user
//         await _httpClient.TestRegisterAndAddJwt(_baseUrl);
//         var jwt = _httpClient.DefaultRequestHeaders.Authorization?.ToString();
//         var securityService = _scopedServiceProvider.GetRequiredService<ISecurityService>();
//         var userId = securityService.VerifyJwtOrThrow(jwt!).Id;
//         Console.WriteLine($"Test running as user: {userId}");
//
//         // Verify user has tasks
//         var dbContext = _scopedServiceProvider.GetRequiredService<MyDbContext>();
//         var userTaskCount = await dbContext.Tickticktasks
//             .Where(t => t.List.UserId == userId)
//             .CountAsync();
//         Console.WriteLine($"User has {userTaskCount} tasks in database");
//
//         // Your existing test code
//         var query = new TaskQueryParams();
//         var response = await _httpClient.GetAsync(
//             TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
//         
//         // Log response
//         var content = await response.Content.ReadAsStringAsync();
//         Console.WriteLine($"Response content: {content}");
//         
//         Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//         var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
//         Assert.That(tasks, Is.Not.Null);
//         Assert.That(tasks!.Count, Is.GreaterThan(0));
//         
//     }
//     
//
//     
//     [Test]
//     public async Task GetTasks_ShouldFilterByCompletion()
//     {
//         // Arrange
//         var query = new TaskQueryParams { IsCompleted = true };
//         await _httpClient.TestRegisterAndAddJwt();
//
//
//         // Act
//         var response = await _httpClient.GetAsync(
//             TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
//         
//         // Assert
//         Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//         var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
//         Assert.That(tasks, Is.Not.Null);
//         Assert.That(tasks!.All(t => t.Completed), Is.True);
//     }
//
//     [Test]
//     public async Task GetTasks_ShouldFilterByDateRange()
//     {
//         // Arrange
//         var query = new TaskQueryParams
//         {
//             DueDateStart = DateTime.UtcNow.AddDays(-7),
//             DueDateEnd = DateTime.UtcNow.AddDays(7)
//         };
//         await _httpClient.TestRegisterAndAddJwt();
//
//
//         // Act
//         var response = await _httpClient.GetFromJsonAsync<List<TickticktaskDto>>(
//             TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
//
//         if (response.Count == 0)
//             throw new Exception("Did not get any tasks");
//         Assert.That(response!.All(t => 
//             t.DueDate >= query.DueDateStart && 
//             t.DueDate <= query.DueDateEnd), Is.True);
//     }
//
//     [Test]
//     public async Task GetTasks_ShouldFilterByPriorityRange()
//     {
//         // Arrange
//         var query = new TaskQueryParams
//         {
//             MinPriority = 2,
//             MaxPriority = 3
//         };
//         await _httpClient.TestRegisterAndAddJwt();
//
//
//         // Act
//         var response = await _httpClient.GetAsync(
//             TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
//         
//         // Assert
//         Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//         var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
//         Assert.That(tasks, Is.Not.Null);
//         Assert.That(tasks!.All(t => t.Priority >= query.MinPriority && t.Priority <= query.MaxPriority), Is.True);
//     }
//
//     [Test]
//     public async Task GetTasks_ShouldFilterBySearchTerm()
//     {
//         // Arrange
//         var query = new TaskQueryParams { SearchTerm = "project" };
//         await _httpClient.TestRegisterAndAddJwt();
//
//         // Act
//         var response = await _httpClient.GetAsync(
//             TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
//         
//         // Assert
//         Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//         var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
//         Assert.That(tasks, Is.Not.Null);
//         Assert.That(tasks!.All(t => 
//             t.Title.ToLower().Contains(query.SearchTerm!.ToLower()) || 
//             t.Description.ToLower().Contains(query.SearchTerm.ToLower())), Is.True);
//     }
//
//     [Test]
//     public async Task GetTasks_ShouldFilterByTags()
//     {
//         // Arrange
//         var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
//         var tagId = ctx.Tags.First().TagId;
//         var query = new TaskQueryParams { TagIds = new List<string> { tagId } };
//         await _httpClient.TestRegisterAndAddJwt();
//
//
//         // Act
//         var response = await _httpClient.GetAsync(
//             "/GetMyTasks");
//         
//         // Assert
//         Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//         List<TickticktaskDto> tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>() ?? throw new Exception("Failed to deserialize tasks");
//         var tasksAsJsonText = JsonSerializer.Serialize(tasks);
//         Console.WriteLine(tasksAsJsonText);
//         Assert.That(tasks, Is.Not.Null);
//         Assert.That(tasks!.Count, Is.GreaterThan(0));
//     }
//
//     [Test]
//     public async Task GetTasks_ShouldFilterByLists()
//     {
//         // Arrange
//         var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
//         var listId = ctx.Tasklists.First().ListId;
//         var query = new TaskQueryParams { ListIds = new List<string> { listId } };
//         await _httpClient.TestRegisterAndAddJwt();
//
//
//         // Act
//         var response = await _httpClient.GetAsync(
//             TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
//         
//         // Assert
//         Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//         var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
//         Assert.That(tasks, Is.Not.Null);
//         Assert.That(tasks!.All(t => t.ListId == listId), Is.True);
//     }
//
//     [Test]
//     [TestCase(nameof(Tickticktask.DueDate), true)]
//     [TestCase(nameof(Tickticktask.Priority), false)]
//     [TestCase(nameof(Tickticktask.CreatedAt), true)]
//     [TestCase(nameof(Tickticktask.CompletedAt), false)]
//     public async Task GetTasks_ShouldOrderCorrectly(string orderByValue, bool isDescending)
//     {
//         // Arrange
//         TaskOrderBy? orderBy;
//         TaskOrderBy.TryParse(orderByValue, out orderBy);
//
//         var query = new TaskQueryParams
//         {
//             OrderBy = orderBy,
//             IsDescending = isDescending
//         };
//
//         await _httpClient.TestRegisterAndAddJwt();
//
//
//         // Act
//         var response = await _httpClient.GetAsync(
//             TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
//
//         // Assert
//         Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//         var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
//         Assert.That(tasks, Is.Not.Null);
//
//
//         IEnumerable<TickticktaskDto> orderedTasks;
//         if (orderBy?.Value is var x1 && x1 == TaskOrderBy.DueDate.Value)
//             orderedTasks = isDescending ? tasks!.OrderByDescending(t => t.DueDate) : tasks!.OrderBy(t => t.DueDate);
//         else if (orderBy?.Value is var x2 && x2 == TaskOrderBy.Priority.Value)
//             orderedTasks = isDescending
//                 ? tasks!.OrderByDescending(t => t.Priority)
//                 : tasks!.OrderBy(t => t.Priority);
//         else if (orderBy?.Value is var x3 && x3 == TaskOrderBy.CreatedAt.Value)
//             orderedTasks = isDescending
//                 ? tasks!.OrderByDescending(t => t.CreatedAt)
//                 : tasks!.OrderBy(t => t.CreatedAt);
//         else if (orderBy?.Value is var x4 && x4 == TaskOrderBy.CompletedAt.Value)
//             orderedTasks = isDescending
//                 ? tasks!.OrderByDescending(t => t.CompletedAt)
//                 : tasks!.OrderBy(t => t.CompletedAt);
//         else
//             orderedTasks = tasks!;
//
//         Assert.That(tasks, Is.EqualTo(orderedTasks.ToList()));
//
//     }
//
//     [Test]
//     public async Task GetTasks_ShouldCombineMultipleFilters()
//     {
//         // Arrange
//         var query = new TaskQueryParams
//         {
//             IsCompleted = false,
//             DueDateStart = DateTime.UtcNow,
//             DueDateEnd = DateTime.UtcNow.AddDays(30),
//             MinPriority = 2,
//             OrderBy = TaskOrderBy.Priority,
//             IsDescending = true
//         };
//         await _httpClient.TestRegisterAndAddJwt();
//
//         // Act
//         var response = await _httpClient.GetAsync(
//             TicktickTaskController.GetMyTasksRoute + query.ToQueryString());
//         
//         // Assert
//         Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
//         var tasks = await response.Content.ReadFromJsonAsync<List<TickticktaskDto>>();
//         Assert.That(tasks, Is.Not.Null);
//         
//         Assert.That(tasks!.All(t => 
//             !t.Completed &&
//             t.DueDate >= query.DueDateStart &&
//             t.DueDate <= query.DueDateEnd &&
//             t.Priority >= query.MinPriority), Is.True);
//         
//         Assert.That(tasks, Is.EqualTo(tasks.OrderByDescending(t => t.Priority)));
//     }
// }