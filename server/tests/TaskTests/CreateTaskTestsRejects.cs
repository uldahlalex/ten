using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Models.Dtos.Requests;
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
    [Arguments("", "asdsa", 1)] //invalid title: empty
    [Arguments("asdsad", "",  1)] //invalid desc: empty
    [Arguments("asdsad", "asdsad",  0)] //invalid priority: not in range
    [Arguments("asdsad", "asdsad", 6)] //invalid priority: not in range
    public async Task CreateTask_ShouldBeRejects_IfDtoDoesNotLiveUpToValidationRequirements(
        string title,
        string description, int priority)
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();


        var request = new CreateTaskRequestDto(
            (ctx.Tasklists.FirstOrDefault() ?? throw new Exception("Could not find any task list")).ListId,
            title,
            description,
            _scopedServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().AddDays(1).UtcDateTime,
            priority);


        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateTask), request);

        // Assert
        if (HttpStatusCode.BadRequest != response.StatusCode)
            throw new Exception("Expected bad request. Received: " + response.StatusCode + " and body :" +
                                await response.Content.ReadAsStringAsync());
    }
}