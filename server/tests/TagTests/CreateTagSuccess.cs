using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.Tag;

public class CreateTagSuccess
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
    public async Task CreateTag()
    {
        var tag = new CreateTagRequestDto("My new tag");

        var response = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateTag), tag);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception("Response status code expected OK but got " + response.StatusCode + " with message: " +
                                await response.Content.ReadAsStringAsync());


        var result = await response.Content.ReadFromJsonAsync<TagDto>() ??
                     throw new Exception("Response content is null");
        Validator.ValidateObject(result, new ValidationContext(result), true);
        if (result.Name != tag.TagName)
            throw new Exception("Expected tag name to be " + tag.TagName + " but got: " + result.Name);
        _ = _scopedServiceProvider.GetRequiredService<MyDbContext>()
            .Tags.First(t => t.TagId == result.TagId); //throws or finds
    }
}