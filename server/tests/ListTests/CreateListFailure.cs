using api.Etc;
using api.Models.Dtos.Requests;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;
using Infrastructure.Postgres.Scaffolding;

namespace tests.List;

public class CreateListFailure : ApiTestBase
{

    [Test]
    public async Task CreateList_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var request = new CreateListRequestDto("");

        // Act & Assert
        try
        {
            await ApiClient.TicktickTask_CreateListAsync(request);
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
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        var lookupId = ids.WorkListId;
        var existingList = ScopedServiceProvider.GetRequiredService<MyDbContext>().Tasklists.First(l => l.ListId == lookupId);
        var request = new CreateListRequestDto(existingList.Name);

        var result = await ApiClient.TicktickTask_CreateListAsync(request);
  
    }
    
    [Test]
    public async Task CreateList_ShouldAllowTakenName_IfItsSomeoneElsesList()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Based on TestDataSeeder, Jane has a list named "Jane's Tasks"
        // John should be able to create a list with the same name since it's someone else's
        var janeListName = "Jane's Tasks";
        var request = new CreateListRequestDto(janeListName);

        var result = await ApiClient.TicktickTask_CreateListAsync(request);
        
        if (result == null)
            throw new Exception("Expected successful list creation but got null result");
    }
}