using api.Models.Dtos.Requests;
using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using tests.Utilities;
using Generated;

namespace tests.TaskTests;

public class UpdateTaskFailure : ApiTestBase
{

    [Test]
    [Arguments("", "asdsa", 1)] //invalid title: empty
    [Arguments("asdsad", "", 1)] //invalid desc: empty
    [Arguments("asdsad", "asdsad", 0)] //invalid priority: not in range
    [Arguments("asdsad", "asdsad",  6)] //invalid priority: not in rage
    public async Task UpdateTask_IsRejected_WhenDtoIsInvalid(
        string title, string description, int priority)

    {
        var ctx = ScopedServiceProvider.GetRequiredService<MyDbContext>();
        var listId = ctx.Tasklists.OrderBy(o => o.CreatedAt).First().ListId;
        var createdAt = ScopedServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().UtcDateTime;
        var taskToUpdate = new Tickticktask(createdAt, listId, "Test title","Test description", createdAt.AddDays(1), 3, false);
        ctx.Tickticktasks.Add(taskToUpdate);
        ctx.SaveChanges();


        var request = new UpdateTaskRequestDto
        (
            taskToUpdate.TaskId,
            title: title,
            description: description,
            dueDate: ScopedServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow().AddDays(1).UtcDateTime,
            priority: priority,
            completed: true,
            listId: ctx.Tasklists.OrderBy(o => o.CreatedAt).Reverse().First().ListId 
        );
        // Act & Assert
        try
        {
            await ApiClient.TicktickTask_UpdateTaskAsync(request);
            throw new Exception("Expected ApiException for bad request but request succeeded");
        }
        catch (ApiException ex) when (ex.StatusCode == 400)
        {
            // Expected - bad request should throw ApiException with 400 status code
        }
    }
}