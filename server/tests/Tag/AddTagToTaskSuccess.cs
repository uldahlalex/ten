using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using api;
using api.Controllers;
using api.Models.Dtos;
using api.Models.Dtos.Requests;
using efscaffold;
using efscaffold.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace tests.Task;

public class AddTagToTaskSuccess
{
    private WebApplication _app = null!;
    private string _baseUrl = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;

    [Before(Test)]
    public async System.Threading.Tasks.Task Setup()
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
    }


    [Test]
    public async System.Threading.Tasks.Task AddTagToTask_CanSuccessfully_AddTagToTask()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        var arbitraryTag = ctx.Tags.Include(t => t.TaskTags).First();
        var taskIdsThatHaveTheTag = arbitraryTag.TaskTags.Select(t => t.TaskId);
        var firstTaskNotAlreadyForTag = ctx.Tickticktasks
            .Include(t => t.TaskTags)
            .First(t => !taskIdsThatHaveTheTag.Contains(t.TaskId));
        

        var dto = new ChangeTaskTagRequestDto()
        {
            TagId = arbitraryTag.TagId,
            TaskId = firstTaskNotAlreadyForTag.TaskId
        };

        var response = await _client.PutAsJsonAsync(_baseUrl + nameof(TicktickTaskController.AddTaskTag), dto);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception("Expected status 200 but got: " + response.StatusCode + " and message: " +
                                await response.Content.ReadAsStringAsync());

        var result = await response.Content.ReadFromJsonAsync<TaskTagDto>() ?? throw new Exception("Response content is null");
        Validator.ValidateObject(result, new ValidationContext(result), true);
        //check exists in db
        _ = ctx.TaskTags.First(t => t.TagId == result.TagId && t.TaskId == firstTaskNotAlreadyForTag.TaskId);
    }
    
}