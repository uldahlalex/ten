using api.Etc;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.Tag;

public class GetMyTags : ApiTestBase
{


    [Test]
    public async Task GetMyTags_CanSuccessfully_ListMyTagsDtos()
    {
        var ids = ScopedServiceProvider.GetRequiredService<ITestDataIds>();
        
        // Based on TestDataSeeder, John should have these specific tags
        var expectedJohnTagIds = new HashSet<string>
        {
            ids.UrgentTagId,     // "Urgent" tag
            ids.BugTagId,        // "Bug" tag  
            ids.FeatureTagId,    // "Feature" tag
            ids.ImportantTagId   // "Important" tag
        };

        var actualTags = await ApiClient.TicktickTask_GetMyTagsAsync();
        
        if (actualTags == null)
            throw new Exception("Response body was null when deserializing to List<TagDto>");
            
        if (actualTags.Count != expectedJohnTagIds.Count)
            throw new Exception($"Expected {expectedJohnTagIds.Count} tags for John but got {actualTags.Count}");
            
        var actualTagIds = actualTags.Select(t => t.TagId).ToHashSet();
        
        if (!expectedJohnTagIds.SetEquals(actualTagIds))
            throw new Exception($"Expected tag IDs: [{string.Join(", ", expectedJohnTagIds)}], but got: [{string.Join(", ", actualTagIds)}]");
            
        // Verify specific tag names from test data
        var urgentTag = actualTags.FirstOrDefault(t => t.TagId == ids.UrgentTagId);
        if (urgentTag?.Name != "Urgent")
            throw new Exception($"Expected Urgent tag to have name 'Urgent' but got '{urgentTag?.Name}'");
            
        var bugTag = actualTags.FirstOrDefault(t => t.TagId == ids.BugTagId);
        if (bugTag?.Name != "Bug")
            throw new Exception($"Expected Bug tag to have name 'Bug' but got '{bugTag?.Name}'");
    }
}