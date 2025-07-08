using System.ComponentModel.DataAnnotations;
using api.Etc;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.List;

public class GetMyLists : ApiTestBase
{
    [Test]
    public async Task GetMyLists_CanSuccessfully_ListMyListDtos()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        var expectedJohnListIds = new HashSet<string>
        {
            ids.WorkListId,     
            ids.PersonalListId, 
            ids.ShoppingListId  
        };

        var actualLists = (await ApiClient.TicktickTask_GetMyListsAsync()).Result;
        
        if (actualLists == null)
            throw new Exception("Response body was null when deserializing to List<TasklistDto>");
            
        if (actualLists.Count != expectedJohnListIds.Count)
            throw new Exception($"Expected {expectedJohnListIds.Count} lists for John but got {actualLists.Count}");
            
        var actualListIds = actualLists.Select(l => l.ListId).ToHashSet();
        
        if (!expectedJohnListIds.SetEquals(actualListIds))
            throw new Exception($"Expected list IDs: [{string.Join(", ", expectedJohnListIds)}], but got: [{string.Join(", ", actualListIds)}]");
        
        Validator.ValidateObject(actualLists.First(), new ValidationContext(actualLists.First()));

    }
}