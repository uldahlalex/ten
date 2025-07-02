using System.Net;
using System.Net.Http.Json;
using api.Controllers;
using api.Etc;
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
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Based on TestDataSeeder, John should have these specific lists
        var expectedJohnListIds = new HashSet<string>
        {
            ids.WorkListId,     // "Work Tasks" list
            ids.PersonalListId, // "Personal Tasks" list  
            ids.ShoppingListId  // "Shopping" list
        };

        var response = await _client.GetAsync(_baseUrl + nameof(TicktickTaskController.GetMyLists));
        
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception($"Expected OK status but got {response.StatusCode}. Response: {await response.Content.ReadAsStringAsync()}");

        var actualLists = await response.Content.ReadFromJsonAsync<List<TasklistDto>>();
        
        if (actualLists == null)
            throw new Exception("Response body was null when deserializing to List<TasklistDto>");
            
        if (actualLists.Count != expectedJohnListIds.Count)
            throw new Exception($"Expected {expectedJohnListIds.Count} lists for John but got {actualLists.Count}");
            
        var actualListIds = actualLists.Select(l => l.ListId).ToHashSet();
        
        if (!expectedJohnListIds.SetEquals(actualListIds))
            throw new Exception($"Expected list IDs: [{string.Join(", ", expectedJohnListIds)}], but got: [{string.Join(", ", actualListIds)}]");
            
        // Verify specific list names from test data
        var workList = actualLists.FirstOrDefault(l => l.ListId == ids.WorkListId);
        if (workList?.Name != "Work Tasks")
            throw new Exception($"Expected Work list to have name 'Work Tasks' but got '{workList?.Name}'");
            
        var personalList = actualLists.FirstOrDefault(l => l.ListId == ids.PersonalListId);
        if (personalList?.Name != "Personal Tasks")
            throw new Exception($"Expected Personal list to have name 'Personal Tasks' but got '{personalList?.Name}'");
    }
}