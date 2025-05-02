using System.Net;
using System.Net.Http.Json;
using api;
using api.Seeder;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace tests;

[TestFixture]
public class CreateTaskTestsRejects
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
        builder.DefaultTestConfig();
        
        _app = builder.Build();
        await Program.ConfigureApp(_app);
        await _app.StartAsync();
        
        _baseUrl = _app.Urls.First() + "/";
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


  

    //Multi case test
    [Test]
    [TestCase("", "asdsa", "2050-04-25T20:22:50.657021Z", 1)] //invalid title: empty
    [TestCase("asdsad", "", "2050-04-25T20:22:50.657021Z", 1)] //invalid desc: empty
    [TestCase("asdsad", "asdsad", "2050-04-25T20:22:50.657021Z", 0)] //invalid priority: not in range
    [TestCase("asdsad", "asdsad", "2050-04-25T20:22:50.657021Z", 6)] //invalid priority: empty
    [TestCase( "asdsad", "asdsad", "2000-04-25T20:22:50.657021Z", 1)] //invalid due date: it is in the past
    public async Task CreateTask_ShouldBeRejects_IfDtoDoesNotLiveUpToValidationRequirements( string title, string description, string timestamp, int priority)
    {
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
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateTask), request);

        // Assert
        if (HttpStatusCode.BadRequest != response.StatusCode)
            throw new Exception("Expected bad request. Received: " + response.StatusCode + " and body :" +
                                await response.Content.ReadAsStringAsync());
    }
}