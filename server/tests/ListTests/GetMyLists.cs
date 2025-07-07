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
        
        // Based on TestDataSeeder, John should have these specific lists
        var expectedJohnListIds = new HashSet<string>
        {
            ids.WorkListId,     // "Work Tasks" list
            ids.PersonalListId, // "Personal Tasks" list  
            ids.ShoppingListId  // "Shopping" list
        };

        var actualLists = await ApiClient.TicktickTask_GetMyListsAsync();
        
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