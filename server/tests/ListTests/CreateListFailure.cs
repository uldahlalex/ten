using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using api.Controllers;
using api.Etc;
using api.Models.Dtos.Requests;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.List;

public class CreateListFailure
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
        _client = ApiTestSetupUtilities.CreateHttpClientWithDefaultTestJwt();
        return Task.CompletedTask;
    }

    [Test]
    public async Task CreateList_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var request = new CreateListRequestDto("");

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateList), request);

        if (response.StatusCode != HttpStatusCode.BadRequest)
            throw new Exception("Expected status 400 but got: " + response.StatusCode + " and message: " +
                                await response.Content.ReadAsStringAsync());
    }
    
    [Test]
    public async Task CreateList_ShouldReturnBadRequest_WhenTakenName()
    {
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Based on TestDataSeeder, John already has a list named "Work Tasks"
        var existingListName = "Work Tasks";
        var request = new CreateListRequestDto(existingListName);

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateList), request);

        if (response.StatusCode != HttpStatusCode.BadRequest)
            throw new Exception($"Expected BadRequest status but got {response.StatusCode}. Response: {await response.Content.ReadAsStringAsync()}");
    }
    
    [Test]
    public async Task CreateList_ShouldAllowTakenName_IfItsSomeoneElsesList()
    {
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Based on TestDataSeeder, Jane has a list named "Jane's Tasks"
        // John should be able to create a list with the same name since it's someone else's
        var janeListName = "Jane's Tasks";
        var request = new CreateListRequestDto(janeListName);

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateList), request);

        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Expected OK status but got {response.StatusCode}. Response: {await response.Content.ReadAsStringAsync()}");
    }
}