using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace tests.List;

public class CreateListSuccess
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
    public async Task CreateList_SuccessfullyCreatesList()
    {
        var dto = new CreateListRequestDto("Example list");
        var actualResponse = await _client.PostAsJsonAsync(_baseUrl + nameof(TicktickTaskController.CreateList), dto);

        if (actualResponse.StatusCode != HttpStatusCode.OK)
            throw new Exception("Expected status code 200, but got " + actualResponse.StatusCode + " with message:" +
                                await actualResponse.Content.ReadAsStringAsync());
        var responseDto = await actualResponse.Content.ReadFromJsonAsync<TasklistDto>() ??
                          throw new Exception("Could not deserialize into " + nameof(TasklistDto));

        if (responseDto.Name != dto.ListName)
            throw new Exception("Did not succesfully create the list with the given name");
        if (Math.Abs((_scopedServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().UtcDateTime - responseDto.CreatedAt).TotalSeconds) > 1)
            throw new Exception("CreatedAt timestamp is not within 1 second of now");
        _ = _scopedServiceProvider.GetRequiredService<MyDbContext>().Tasklists
            .First(l => l.ListId == responseDto.ListId); //throws or finds
    }
}