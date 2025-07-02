using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Mappers;
using api.Models.Dtos.Responses;
using FluentAssertions;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.List;

public class GetMyLists
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
    public async Task GetMyLists_CanSuccessfully_ListMyListDtos()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();


        var response = await _client.GetAsync(_baseUrl + nameof(TicktickTaskController.GetMyLists));
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception("Expected status 200 but got: " + response.StatusCode + " and message: " +
                                await response.Content.ReadAsStringAsync());

        var dtos = await response.Content.ReadFromJsonAsync<List<TasklistDto>>();
        if (dtos.Count != 3)
            throw new Exception("Expected exactly 3 lists for the given user, but got: " + dtos.Count);
         }
}