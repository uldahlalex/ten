using api.Etc;
using api.Models.Dtos.Requests;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

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
        catch (ApiException ex) 
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


        try
        {
            await ApiClient.TicktickTask_CreateListAsync(request);
            throw new Exception("It should not be possible to create a list with an already taken name for the user");
        }
        catch (ApiException e)
        {
            // Expected - bad request should throw ApiException with 400 status code

        }
       

    }
    

}