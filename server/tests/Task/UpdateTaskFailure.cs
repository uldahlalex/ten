using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using api;
using api.Controllers;
using api.Extensions.Mappers;
using api.Models.Dtos;
using api.Models.Dtos.Requests;
using efscaffold;
using efscaffold.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.Task;

public class UpdateTaskFailure
{
    private WebApplication _app = null!;
    private string _baseUrl = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;

    [Before(Test)]
    public async System.Threading.Tasks.Task Setup()
    {
        var builder = WebApplication.CreateBuilder();
        Program.ConfigureServices(builder);
        builder.DefaultTestConfig();

        _app = builder.Build();
        Program.ConfigureApp(_app);
        await _app.StartAsync();

        _baseUrl = _app.Urls.First() + "/";
        _scopedServiceProvider = _app.Services.CreateScope().ServiceProvider;
        _client = new HttpClient();
        await _client.TestRegisterAndAddJwt(_baseUrl);
    }
    [Test]
    [Arguments("", "asdsa", "2050-04-25T20:22:50.657021Z", 1)] //invalid title: empty
    [Arguments("asdsad", "", "2050-04-25T20:22:50.657021Z", 1)] //invalid desc: empty
    [Arguments("asdsad", "asdsad", "2050-04-25T20:22:50.657021Z", 0)] //invalid priority: not in range
    [Arguments("asdsad", "asdsad", "2050-04-25T20:22:50.657021Z", 6)] //invalid priority: not in rage
    [Arguments("asdsad", "asdsad", "2000-04-25T20:22:50.657021Z", 1)] //invalid due date: it is in the past
    public async System.Threading.Tasks.Task UpdateTask_IsRejected_WhenDtoIsInvalid(
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
        {
            Id = taskToUpdate.TaskId,
            Title = title,
            Description = description,
            DueDate = DateTime.Parse(timestamp).ToUniversalTime(),
            Priority = priority,
            Completed = true,
            ListId = ctx.Tasklists.OrderBy(o => o.CreatedAt).Reverse().FirstOrDefault()
                .ListId //moving to a different list
        };
        var response = await _client.PatchAsJsonAsync(_baseUrl + nameof(TicktickTaskController.UpdateTask), request);
        if(response.StatusCode!=HttpStatusCode.BadRequest)
            throw new Exception("Expected status code 400 for bad request. Got:  "+response.StatusCode+" and message: "+await response.Content.ReadAsStringAsync());
    }
    
}