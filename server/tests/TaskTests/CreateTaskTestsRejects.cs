using api.Models.Dtos.Requests;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.TaskTests;

public class CreateTaskTestsRejects : ApiTestBase
{
    

    
    //Multi case test
    [Test]
    [Arguments("", "asdsa", 1)] //invalid title: empty
    [Arguments("asdsad", "",  1)] //invalid desc: empty
    [Arguments("asdsad", "asdsad",  0)] //invalid priority: not in range
    [Arguments("asdsad", "asdsad", 6)] //invalid priority: not in range
    public async Task CreateTask_ShouldBeRejects_IfDtoDoesNotLiveUpToValidationRequirements(
        string title,
        string description, int priority)
    {
        var ctx = ScopedServiceProvider.GetRequiredService<MyDbContext>();


        var request = new CreateTaskRequestDto(
            (ctx.Tasklists.FirstOrDefault() ?? throw new Exception("Could not find any task list")).ListId,
            title,
            description,
            ScopedServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().AddDays(1).UtcDateTime,
            priority);


        // Act & Assert
        var result = await ApiClient.TicktickTask_CreateTaskAsync(request);
        if (result.StatusCode != 400)
            throw new Exception($"Expected status code 400 but got {result.StatusCode}");
    }
}