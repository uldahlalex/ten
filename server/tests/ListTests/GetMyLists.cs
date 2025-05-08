using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Mappers;
using api.Models.Dtos.Responses;
using efscaffold;
using FluentAssertions;
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
        _client = _app.CreateHttpClientWithDefaultTestJwt();
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
        if (dtos.Count != 10)
            throw new Exception("Expected exactly 10 lists for the given user, but got: " + dtos.Count);
        var orderedActual = dtos.OrderBy(d => d.ListId);
        var orderedExpected = ctx.Tasklists.OrderBy(t => t.ListId).Select(t => t.ToDto());
        orderedActual.Should().BeEquivalentTo(orderedExpected);
    }
}