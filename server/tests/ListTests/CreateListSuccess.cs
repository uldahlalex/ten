using System.ComponentModel.DataAnnotations;
using api.Etc;
using api.Models.Dtos.Requests;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.ListTests;

public class CreateListSuccess : ApiTestBase
{

    [Test]
    public async Task CreateList_SuccessfullyCreatesList()
    {
        var dto = new CreateListRequestDto("Example list");
        var responseDto = (await ApiClient.TicktickTask_CreateListAsync(dto)).Result;

        if (responseDto.Name != dto.ListName)
            throw new Exception("Did not succesfully create the list with the given name");
        if (Math.Abs((ScopedServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().UtcDateTime - responseDto.CreatedAt).TotalSeconds) > 1)
            throw new Exception("CreatedAt timestamp is not within 1 second of now");
        _ = ScopedServiceProvider.GetRequiredService<MyDbContext>().Tasklists
            .First(l => l.ListId == responseDto.ListId); 
    }
    
    [Test]
    public async Task CreateList_ShouldAllowTakenName_IfItsSomeoneElsesList()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        var db = ScopedServiceProvider.GetRequiredService<MyDbContext>();
        var janesList = db.Tasklists.First(u => u.UserId == ids.JaneId);
    
        var request = new CreateListRequestDto(listName:janesList.Name);

        var result = (await ApiClient.TicktickTask_CreateListAsync(request)).Result;
        
        Validator.ValidateObject(result, new ValidationContext(result));
        
    }
}