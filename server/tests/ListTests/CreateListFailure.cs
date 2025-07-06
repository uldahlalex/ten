using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using api.Controllers;
using api.Etc;
using api.Models.Dtos.Requests;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Generated;

namespace tests.List;

public class CreateListFailure
{
    private WebApplication _app = null!;
    private string _baseUrl = null!;
    private HttpClient _client = null!;
    private IServiceProvider _scopedServiceProvider = null!;
    private IApiClient _apiClient = null!;

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
        _apiClient = new ApiClient(_baseUrl, _client);
        return Task.CompletedTask;
    }

    [Test]
    public async Task CreateList_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var request = new CreateListRequestDto("");

        // Act & Assert
        try
        {
            await _apiClient.TicktickTask_CreateListAsync(request);
            throw new Exception("Expected ApiException for bad request but request succeeded");
        }
        catch (ApiException ex) when (ex.StatusCode == 400)
        {
            // Expected - bad request should throw ApiException with 400 status code
        }
    }
    
    [Test]
    public async Task CreateList_ShouldReturnBadRequest_WhenTakenName()
    {
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Based on TestDataSeeder, John already has a list named "Work Tasks"
        var existingListName = "Work Tasks";
        var request = new CreateListRequestDto(existingListName);

        // Act & Assert
        try
        {
            await _apiClient.TicktickTask_CreateListAsync(request);
            throw new Exception("Expected ApiException for duplicate name but request succeeded");
        }
        catch (ApiException ex) when (ex.StatusCode == 400)
        {
            // Expected - duplicate name should throw ApiException with 400 status code
        }
    }
    
    [Test]
    public async Task CreateList_ShouldAllowTakenName_IfItsSomeoneElsesList()
    {
        var ids = _scopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Based on TestDataSeeder, Jane has a list named "Jane's Tasks"
        // John should be able to create a list with the same name since it's someone else's
        var janeListName = "Jane's Tasks";
        var request = new CreateListRequestDto(janeListName);

        var result = await _apiClient.TicktickTask_CreateListAsync(request);
        
        if (result == null)
            throw new Exception("Expected successful list creation but got null result");
    }
}