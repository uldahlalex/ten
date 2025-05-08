using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using api;
using api.Controllers;
using api.Models.Dtos.Requests;
using efscaffold;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.Task;

public class CreateListFailure
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
    [Arguments("")]//invalid string for listname
    [Arguments("Work")]//taken name
    public async System.Threading.Tasks.Task CreateList_ShouldReturnBadRequest_WhenInvalidRequest(string listName)
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();

        var request = new CreateListRequestDto()
        {
            ListName = listName
        };

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateList), request);

        if (response.StatusCode != HttpStatusCode.BadRequest)
            throw new Exception("Expected status 400 but got: " + response.StatusCode + " and message: " +
                                await response.Content.ReadAsStringAsync());
    }
    
}