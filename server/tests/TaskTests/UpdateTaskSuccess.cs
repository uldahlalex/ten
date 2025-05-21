using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using api.Controllers;
using api.Mappers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using efscaffold;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.TaskTests;

public class UpdateTaskSuccess
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
    public async Task UpdateTask_CanSuccessfullyUpdateTask()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var taskToUpdate = new Tickticktask
        {
            Title = "Test task",
            Description = "Test description",
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = 1,
            Completed = false,
            ListId = ctx.Tasklists.OrderBy(o => o.CreatedAt).FirstOrDefault().ListId,
            TaskId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };
        ctx.Tickticktasks.Add(taskToUpdate);
        ctx.SaveChanges();


        var request = new UpdateTaskRequestDto
        (
            id: taskToUpdate.TaskId,
            title: "Updated Title",
            description: "Updated Description",
            dueDate: DateTime.UtcNow.AddDays(10).ToUniversalTime(),
            priority: 3,
            completed: true,
            listId: ctx.Tasklists.OrderBy(o => o.CreatedAt).Reverse().FirstOrDefault()
                .ListId //moving to a different list
        );
        var response = await _client.PatchAsJsonAsync(_baseUrl + nameof(TicktickTaskController.UpdateTask), request);
        response.EnsureSuccessStatusCode();
        var updatedTask = await response.Content.ReadFromJsonAsync<TickticktaskDto>();

        var taskInDb = ctx.Tickticktasks.First(t => t.TaskId == request.Id).ToDto();
        Validator.ValidateObject(taskInDb, new ValidationContext(taskInDb), true);
        if (request.Title != updatedTask.Title)
            throw new Exception("Title was not updated");
        if (request.Description != updatedTask.Description)
            throw new Exception("Description was not updated");
        if (request.DueDate != updatedTask.DueDate)
            throw new Exception("DueDate was not updated");
        if (request.Priority != updatedTask.Priority)
            throw new Exception("Priority was not updated");
        if (request.Completed != updatedTask.Completed)
            throw new Exception("Expected completed to be " + request.Completed + " but was " + updatedTask.Completed);
        if (request.ListId != updatedTask.ListId)
            throw new Exception("ListId was not updated");
        if (updatedTask.CompletedAt == null)
            throw new Exception("CompletedAt timestamp was not added when the task was marked as completed");
    }
}