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
        var existingList = ScopedServiceProvider.GetRequiredService<MyDbContext>()
            .Tasklists.First(l => l.ListId == lookupId && l.UserId == ids.JohnId);
        var request = new CreateListRequestDto(existingList.Name);

        try
        {
            await ApiClient.TicktickTask_CreateListAsync(request);
            throw new Exception("Expected 400 status code failure");
        }
        catch (ApiException)
        {
            //Success
        }
  
    }
    
    [Test]
    public async Task CreateList_ShouldAllowTakenName_IfItsSomeoneElsesList()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        var janesFirstList = ScopedServiceProvider.GetRequiredService<MyDbContext>()
            .Tasklists.First(u => u.UserId == ids.JaneId);
      
        var request = new CreateListRequestDto(janesFirstList.Name);

        var result = await ApiClient.TicktickTask_CreateListAsync(request);
        
        if (result == null)
            throw new Exception("Expected successful list creation but got null result");
    }
}