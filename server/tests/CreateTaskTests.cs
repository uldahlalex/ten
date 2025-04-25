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

    private HttpClient _httpClient;
    private IServiceProvider _scopedServiceProvider;


    [Test]
    public async Task CreateTask_ShouldReturnOk_WhenValidRequest()
    {
        await _httpClient.TestRegisterAndAddJwt();
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        Console.WriteLine("tasklists: " + ctx.Tasklists.ToList().Count);

        var request = new CreateTaskRequestDto
        {
            ListId = ctx.Tasklists.First().ListId,
            Title = "Test Task",
            Description = "Test Description",
            DueDate = DateTime.UtcNow,
            Priority = 1
            // TaskTagsDtos = new List<TaskTagDto>()
            // {
            //     new TaskTagDto(){ TagId = ctx.Tags.First().TagId }
            // },
        };


        // Act
        var response = await _httpClient.PostAsJsonAsync(TicktickTaskController.CreateTaskRoute, request);

        // Assert
        if (HttpStatusCode.OK != response.StatusCode)
            throw new Exception("Did not get status 200. Received: " + response.StatusCode + " and body :" +
                                await response.Content.ReadAsStringAsync());
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

        var request = new
        {
            ListId = "test-list-id",
            Title = "",
            Description = "",
            DueDate = DateTime.UtcNow,
            Priority = 1,
            TaskTagsDtos = new[]
            {
                new { TagId = "test-tag-id" }
            }
        };


        // Act
        var response = await _httpClient.PostAsJsonAsync(TicktickTaskController.CreateTaskRoute, request);

        // Assert
        if (HttpStatusCode.BadRequest != response.StatusCode)
            throw new Exception("Expected bad request. Received: " + await response.Content.ReadAsStringAsync());
    }
}