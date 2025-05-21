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
    [Arguments("", "asdsa", "2050-04-25T20:22:50.657021Z", 1)] //invalid title: empty
    [Arguments("asdsad", "", "2050-04-25T20:22:50.657021Z", 1)] //invalid desc: empty
    [Arguments("asdsad", "asdsad", "2050-04-25T20:22:50.657021Z", 0)] //invalid priority: not in range
    [Arguments("asdsad", "asdsad", "2050-04-25T20:22:50.657021Z", 6)] //invalid priority: not in rage
    public async Task UpdateTask_IsRejected_WhenDtoIsInvalid(
        string title, string description, string timestamp, int priority)

    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var taskToUpdate = new Tickticktask
        {
            Title = "title",
            Description = "description",
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = 3,
            Completed = false,
            ListId = ctx.Tasklists.OrderBy(o => o.CreatedAt).First().ListId,
            TaskId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };
        ctx.Tickticktasks.Add(taskToUpdate);
        ctx.SaveChanges();


        var request = new UpdateTaskRequestDto
        (
            taskToUpdate.TaskId,
            title: title,
            description: description,
            dueDate: DateTime.Parse(timestamp).ToUniversalTime(),
            priority: priority,
            completed: true,
            listId: ctx.Tasklists.OrderBy(o => o.CreatedAt).Reverse().FirstOrDefault()
                .ListId //moving to a different list
        );
        var response = await _client.PatchAsJsonAsync(_baseUrl + nameof(TicktickTaskController.UpdateTask), request);
        if (response.StatusCode != HttpStatusCode.BadRequest)
            throw new Exception("Expected status code 400 for bad request. Got:  " + response.StatusCode +
                                " and message: " + await response.Content.ReadAsStringAsync());
    }
}