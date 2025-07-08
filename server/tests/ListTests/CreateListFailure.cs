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

        var result = await ApiClient.TicktickTask_CreateListAsync(request);
        if (result.StatusCode != 400)
            throw new Exception($"Expected status code 400 but got {result.StatusCode}");
    
    }
    
    [Test]
    public async Task CreateList_ShouldReturnBadRequest_WhenTakenName()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        var lookupId = ids.WorkListId;
        var existingList = ScopedServiceProvider.GetRequiredService<MyDbContext>()
            .Tasklists.First(l => l.ListId == lookupId && l.UserId == ids.JohnId);
        var request = new CreateListRequestDto(existingList.Name);


        var result = await ApiClient.TicktickTask_CreateListAsync(request);
        if (result.StatusCode != 400)
            throw new Exception($"Expected status code 400 for duplicate name but got {result.StatusCode}");
       

    }
    

}