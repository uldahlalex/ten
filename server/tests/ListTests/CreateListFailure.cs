using System.Net;
using System.Net.Http.Json;
using api.Controllers;
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
        _client = _app.CreateHttpClientWithDefaultTestJwt();
        return Task.CompletedTask;
    }

    [Test]
    [Arguments("")] //invalid string for listname
    [Arguments("Work")] //taken name
    public async Task CreateList_ShouldReturnBadRequest_WhenInvalidRequest(string listName)
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();

        var request = new CreateListRequestDto(listName);

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateList), request);

        if (response.StatusCode != HttpStatusCode.BadRequest)
            throw new Exception("Expected status 400 but got: " + response.StatusCode + " and message: " +
                                await response.Content.ReadAsStringAsync());
    }
}