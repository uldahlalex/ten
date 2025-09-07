using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;

namespace api.Services;

public class TaskService(IUserDataService userDataService, ITaskDataService taskDataService) : ITaskService
{
    public async Task<List<TickticktaskDto>> GetMyTasks(TaskFilteringAndSorting parameters, string requesterId)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        return await taskDataService.GetMyTasks(parameters, requesterId);
    }

    public async Task<TickticktaskDto> CreateTask(CreateTaskRequestDto dto, string requesterId)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        return await taskDataService.CreateTask(dto, requesterId);
    }

    public async Task<TickticktaskDto> UpdateTask(UpdateTaskRequestDto dto, string requesterId)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        return await taskDataService.UpdateTask(dto, requesterId);
    }

    public async Task DeleteTask(string taskId, string requesterId)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        await taskDataService.DeleteTask(taskId, requesterId);
    }

    public async Task<List<TasklistDto>> GetMyLists(string requesterId)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        return await taskDataService.GetMyLists(requesterId);
    }

    public async Task<List<TagDto>> GetMyTags(string requesterId)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        return await taskDataService.GetMyTags(requesterId);
    }

    public async Task<TasklistDto> CreateList(string requesterId, CreateListRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        return await taskDataService.CreateList(requesterId, dto);
    }

    public async Task<TagDto> CreateTag(string requesterId, CreateTagRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        return await taskDataService.CreateTag(requesterId, dto);
    }

    public async Task<TasklistDto> UpdateList(string requesterId, UpdateListRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        return await taskDataService.UpdateList(requesterId, dto);
    }

    public async Task<TagDto> UpdateTag(string requesterId, UpdateTagRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        return await taskDataService.UpdateTag(requesterId, dto);
    }

    public async Task DeleteListWithAllTasks(string listId, string requesterId)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        await taskDataService.DeleteListWithAllTasks(listId, requesterId);
    }

    public async Task DeleteTag(string tagId, string requesterId)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        await taskDataService.DeleteTag(tagId, requesterId);
    }

    public async Task<TaskTagDto> AddTagToTask(string requesterId, ChangeTaskTagRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        return await taskDataService.AddTagToTask(requesterId, dto);
    }

    public async Task RemoveTaskTag(string requesterId, ChangeTaskTagRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(requesterId))
            throw new Exception("User does not exist");
            
        await taskDataService.RemoveTaskTag(requesterId, dto);
    }
}