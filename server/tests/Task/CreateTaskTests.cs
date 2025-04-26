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
                builder.ConfigureServices(services => { services.DefaultTestConfig(
                useTestContainer:false); });
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
            DueDate = DateTime.Parse("2050-04-25T20:22:50.657021Z").ToUniversalTime(),
            Priority = 1
   
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
        Validator.ValidateObject(responseBodyAsDto, new ValidationContext(responseBodyAsDto), true);
    }

    //Multi case test
    [Test]
    [TestCase("", "asdsa", "2050-04-25T20:22:50.657021Z", 1)] //invalid title: empty
    [TestCase("asdsad", "", "2050-04-25T20:22:50.657021Z", 1)] //invalid desc: empty
    [TestCase("asdsad", "asdsad", "2050-04-25T20:22:50.657021Z", 0)] //invalid priority: not in range
    [TestCase("asdsad", "asdsad", "2050-04-25T20:22:50.657021Z", 6)] //invalid priority: empty
    [TestCase( "asdsad", "asdsad", "2000-04-25T20:22:50.657021Z", 1)] //invalid due date: it is in the past
    public async Task CreateTask_ShouldBeRejects_IfDtoDoesNotLiveUpToValidationRequirements( string title, string description, string timestamp, int priority)
    {
        await _httpClient.TestRegisterAndAddJwt();
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
 

        var request = new CreateTaskRequestDto()
        {
            ListId = (ctx.Tasklists.FirstOrDefault() ?? throw new Exception("Could not find any task list")).ListId,
            Title = title,
            Description = description,
            DueDate = DateTime.Parse(timestamp).ToUniversalTime(),
            Priority = priority
        };


        // Act
        var response = await _httpClient.PostAsJsonAsync(TicktickTaskController.CreateTaskRoute, request);

        // Assert
        if (HttpStatusCode.BadRequest != response.StatusCode)
            throw new Exception("Expected bad request. Received: " + response.StatusCode + " and body :" +
                                await response.Content.ReadAsStringAsync());
    }
}