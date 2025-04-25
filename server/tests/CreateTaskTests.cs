using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using api;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace tests;

[TestFixture]
public class CreateTaskTests
{
    
    private HttpClient _httpClient;
    private IServiceProvider _scopedServiceProvider;

    [SetUp]
    public void Setup()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services => { services.DefaultTestConfig(); });
            });

        _httpClient = factory.CreateClient();
        _scopedServiceProvider = factory.Services.CreateScope().ServiceProvider;
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

 
    [Test]
    public async Task CreateTask_ShouldReturnOk_WhenValidRequest()
    {
        
        await _httpClient.TestRegisterAndAddJwt();
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
    
        var request = new CreateTaskRequestDto()
        {
            
            ListId = ctx.Tasklists.First().ListId,
            Title = "Test Task",
            Description = "Test Description",
            DueDate = DateTime.UtcNow,
            Priority = 1,
            TaskTagsDtos = new List<TaskTagDto>()
            {
                new TaskTagDto(){ TagId = ctx.Tags.First().TagId }
            },
            
        };

        ctx.SaveChanges();

        // Act
        var response = await _httpClient.PostAsJsonAsync(TicktickTaskController.CreateTaskRoute, request);

        // Assert
        if(HttpStatusCode.OK != response.StatusCode)
            throw new Exception("Did not get success status code");
        var responseBodyAsDto = await response.Content.ReadFromJsonAsync<TickticktaskDto>() ??
                                throw new Exception("Could not deserialize to " + nameof(TickticktaskDto));
        // Assert the default data validation put on response DTO class are all valid (throws exc if not)
        Validator.TryValidateObject(responseBodyAsDto, new ValidationContext(responseBodyAsDto), null);
    }
    
    [Test]
    public async Task CreateTask_ShouldFail_IfDtoDoesNotLiveUpToValidationRequirements()
    {
        
        await _httpClient.TestRegisterAndAddJwt();
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        ctx.Tags.Add(new Tag()
        {
            TagId = "test-tag-id",
            Name = "Test Tag",
            TaskTags = [],
            CreatedAt = DateTime.UtcNow,
            UserId = ctx.Users.First().UserId
        });
        var request = new
        {
            ListId = "test-list-id",
            Title = "Test Task",
            Description = "Test Description",
            DueDate = DateTime.UtcNow,
            Priority = 1,
            TaskTagsDtos = new[]
            {
                new { TagId = "test-tag-id" }
            }
        };
        ctx.Tasklists.Add(new Tasklist()
        {
            ListId = Guid.NewGuid().ToString(),
            Name = "Test List",
            CreatedAt = DateTime.UtcNow,
            UserId = ctx.Users.First().UserId,
            Tickticktasks = new List<Tickticktask>() {}
        });
        ctx.SaveChanges();


        // Act
        var response = await _httpClient.PostAsJsonAsync(TicktickTaskController.CreateTaskRoute, request);

        // Assert
        if(HttpStatusCode.OK != response.StatusCode)
            throw new Exception("Did not get success status code");
        var responseBodyAsDto = await response.Content.ReadFromJsonAsync<TickticktaskDto>() ??
                                throw new Exception("Could not deserialize to " + nameof(TickticktaskDto));
        // Assert the default data validation put on response DTO class are all valid (throws exc if not)
        Validator.TryValidateObject(responseBodyAsDto, new ValidationContext(responseBodyAsDto), null);
        throw new NotImplementedException();
    }

    
}