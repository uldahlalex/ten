using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Mappers;
using api.Models.Dtos.Responses;
using efscaffold;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace tests.Tag;

public class GetMyTags
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
    public async Task GetMyTags_CanSuccessfully_ListMyTagsDtos()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();


        var response = await _client.GetAsync(_baseUrl + nameof(TicktickTaskController.GetMyTags));
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception("Expected status 200 but got: " + response.StatusCode + " and message: " +
                                await response.Content.ReadAsStringAsync());

        var dtos = await response.Content.ReadFromJsonAsync<List<TagDto>>();
        if (dtos.Count != 15)
            throw new Exception("Expected exactly 15 tags for the given user with given tags, but got: " + dtos.Count);
        var orderedActual = dtos.OrderBy(d => d.TagId);
        var orderedExpected = ctx.Tags.OrderBy(t => t.TagId).Select(t => t.ToDto());
        orderedActual.Should().BeEquivalentTo(orderedExpected);
    }
}