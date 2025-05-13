using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Models.Dtos.Requests;
using efscaffold;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.TaskTests;

public class CreateTaskTestsRejects
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
        _client = _app.CreateHttpClientWithDefaultTestJwt();
        return Task.CompletedTask;
    }

    //Multi case test
    [Test]
    [Arguments("", "asdsa", "2050-04-25T20:22:50.657021Z", 1)] //invalid title: empty
    [Arguments("asdsad", "", "2050-04-25T20:22:50.657021Z", 1)] //invalid desc: empty
    [Arguments("asdsad", "asdsad", "2050-04-25T20:22:50.657021Z", 0)] //invalid priority: not in range
    [Arguments("asdsad", "asdsad", "2050-04-25T20:22:50.657021Z", 6)] //invalid priority: not in range
    [Arguments("asdsad", "asdsad", "2000-04-25T20:22:50.657021Z", 1)] //invalid due date: it is in the past
    public async Task CreateTask_ShouldBeRejects_IfDtoDoesNotLiveUpToValidationRequirements(
        string title,
        string description, string timestamp, int priority)
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();


        var request = new CreateTaskRequestDto
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