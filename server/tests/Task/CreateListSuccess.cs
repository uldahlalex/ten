using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using api;
using api.Controllers;
using api.Models.Dtos;
using api.Models.Dtos.Requests;
using efscaffold;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.Task;

public class CreateListSuccess
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
        _client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpZCI6InVzZXItMSJ9.LUnCy-TvtvyRhFyyg2qFFwhGMLYAFFFqrKEcBLFAf1Q");
    }


    [Test]
    public async System.Threading.Tasks.Task CreateList_SuccessfullyCreatesList()
    {
        var dto = new CreateListRequestDto()
        {
            ListName = "Example list"
        };
        var actualResponse = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateList), dto);

        if (actualResponse.StatusCode != HttpStatusCode.OK)
            throw new Exception("Expected status code 200, but got " + actualResponse.StatusCode + " with message:" +
                                await actualResponse.Content.ReadAsStringAsync());
        var responseDto = await actualResponse.Content.ReadFromJsonAsync<TasklistDto>() ??
                          throw new Exception("Could not deserialize into " + nameof(TasklistDto));

        if (responseDto.Name != dto.ListName)
            throw new Exception("Did not succesfully create the list with the given name");
        if (Math.Abs((DateTime.UtcNow - responseDto.CreatedAt).TotalSeconds) > 1)
            throw new Exception("CreatedAt timestamp is not within 1 second of now");
        _ = _scopedServiceProvider.GetRequiredService<MyDbContext>().Tasklists
            .First(l => l.ListId == responseDto.ListId); //throws or finds
    }
    
}