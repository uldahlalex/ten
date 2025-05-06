using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using api;
using api.Controllers;
using api.Models.Dtos;
using api.Models.Dtos.Requests;
using efscaffold;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace tests.Task;

public class CreateTaskTestsSuccess
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
    public async System.Threading.Tasks.Task CreateTask_ShouldReturnOk_WhenValidRequest()
    {
        var logger = _scopedServiceProvider.GetRequiredService<ILogger<string>>();
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();

        // _scopedServiceProvider.GetRequiredService<ISeeder>().CreateEnvironment(ctx);
        var request = new CreateTaskRequestDto
        {
            ListId = ctx.Tasklists.First().ListId,
            Title = "Test Task",
            Description = "Test Description",
            DueDate = DateTime.Parse("2050-04-25T20:22:50.657021Z").ToUniversalTime(),
            Priority = 1
        };


        // Act
        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateTask), request);

        // Assert
        if (HttpStatusCode.OK != response.StatusCode)
            throw new Exception("Did not get status 200. Received: " + response.StatusCode + " and body :" +
                                await response.Content.ReadAsStringAsync());
        var responseBodyAsDto = await response.Content.ReadFromJsonAsync<TickticktaskDto>() ??
                                throw new Exception("Could not deserialize to " + nameof(TickticktaskDto));
        // Assert the default data validation put on response DTO class are all valid (throws exc if not)
        Validator.ValidateObject(responseBodyAsDto, new ValidationContext(responseBodyAsDto), true);
        var lookup = ctx.Tickticktasks.First(t => t.TaskId == responseBodyAsDto.TaskId);
        Validator.ValidateObject(lookup, new ValidationContext(lookup), true);
    }
}