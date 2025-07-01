using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Models.Dtos.Requests;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.TaskTests;

public class UpdateTaskFailure
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

    [Test]
    [Arguments("", "asdsa", 1)] //invalid title: empty
    [Arguments("asdsad", "", 1)] //invalid desc: empty
    [Arguments("asdsad", "asdsad", 0)] //invalid priority: not in range
    [Arguments("asdsad", "asdsad",  6)] //invalid priority: not in rage
    public async Task UpdateTask_IsRejected_WhenDtoIsInvalid(
        string title, string description, int priority)

    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var listId = ctx.Tasklists.OrderBy(o => o.CreatedAt).First().ListId;
        var createdAt = _scopedServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().UtcDateTime;
        var taskToUpdate = new Tickticktask(createdAt, listId, "Test title","Test description", createdAt.AddDays(1), 3, false);
        ctx.Tickticktasks.Add(taskToUpdate);
        ctx.SaveChanges();


        var request = new UpdateTaskRequestDto
        (
            taskToUpdate.TaskId,
            title: title,
            description: description,
            dueDate: _scopedServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().AddDays(1).UtcDateTime,
            priority: priority,
            completed: true,
            listId: ctx.Tasklists.OrderBy(o => o.CreatedAt).Reverse().First().ListId 
        );
        var response = await _client.PatchAsJsonAsync(_baseUrl + nameof(TicktickTaskController.UpdateTask), request);
        if (response.StatusCode != HttpStatusCode.BadRequest)
            throw new Exception("Expected status code 400 for bad request. Got:  " + response.StatusCode +
                                " and message: " + await response.Content.ReadAsStringAsync());
    }
}