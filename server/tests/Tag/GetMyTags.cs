using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using api;
using api.Controllers;
using api.Extensions.Mappers;
using api.Models.Dtos;
using efscaffold;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace tests.Task;

public class GetMyTags
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
    public async System.Threading.Tasks.Task GetMyTags_CanSuccessfully_ListMyTagsDtos()
    {
        var ctx = _scopedServiceProvider.GetRequiredService<MyDbContext>();
        

        var response = await _client.GetAsync(_baseUrl + nameof(TicktickTaskController.GetMyTags));
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception("Expected status 200 but got: " + response.StatusCode + " and message: " +
                                await response.Content.ReadAsStringAsync());

        var dtos = await response.Content.ReadFromJsonAsync<List<TagDto>>();
        if (dtos.Count != 15)
            throw new Exception("Expected exactly 15 tags for the given user with given tags, but got: "+dtos.Count);
        var orderedActual = dtos.OrderBy(d => d.TagId);
        var orderedExpected = ctx.Tags.OrderBy(t => t.TagId).Select(t => t.ToDto());
        orderedActual.Should().BeEquivalentTo(orderedExpected);
    }
}