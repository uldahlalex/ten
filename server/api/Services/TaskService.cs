using api.Models;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;

namespace api.Services;

public class TaskService(IUserDataService userDataService, ITaskDataService taskDataService) : ITaskService
{
    public async Task<List<TickticktaskDto>> GetMyTasks(GetTasksFilterAndOrderParameters parameters, JwtClaims jwtClaims)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(jwtClaims.Id))
            throw new Exception("User does not exist");
            
        return await taskDataService.GetMyTasks(parameters, jwtClaims.Id);
    }

    public async Task<TickticktaskDto> CreateTask(CreateTaskRequestDto dto, JwtClaims jwtClaims)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(jwtClaims.Id))
            throw new Exception("User does not exist");
            
        return await taskDataService.CreateTask(dto, jwtClaims.Id);
    }

    public async Task<TickticktaskDto> UpdateTask(UpdateTaskRequestDto dto, JwtClaims claims)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        return await taskDataService.UpdateTask(dto, claims.Id);
    }

    public async Task DeleteTask(string taskId, JwtClaims claims)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        await taskDataService.DeleteTask(taskId, claims.Id);
    }

    public async Task<List<TasklistDto>> GetMyLists(JwtClaims claims)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        return await taskDataService.GetMyLists(claims.Id);
    }

    public async Task<List<TagDto>> GetMyTags(JwtClaims claims)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        return await taskDataService.GetMyTags(claims.Id);
    }

    public async Task<TasklistDto> CreateList(JwtClaims claims, CreateListRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        return await taskDataService.CreateList(claims.Id, dto);
    }

    public async Task<TagDto> CreateTag(JwtClaims claims, CreateTagRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        return await taskDataService.CreateTag(claims.Id, dto);
    }

    public async Task<TasklistDto> UpdateList(JwtClaims claims, UpdateListRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        return await taskDataService.UpdateList(claims.Id, dto);
    }

    public async Task<TagDto> UpdateTag(JwtClaims claims, UpdateTagRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        return await taskDataService.UpdateTag(claims.Id, dto);
    }

    public async Task DeleteListWithAllTasks(string listId, JwtClaims claims)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        await taskDataService.DeleteListWithAllTasks(listId, claims.Id);
    }

    public async Task DeleteTag(string tagId, JwtClaims claims)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        await taskDataService.DeleteTag(tagId, claims.Id);
    }

    public async Task<TaskTagDto> AddTagToTask(JwtClaims claims, ChangeTaskTagRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        return await taskDataService.AddTagToTask(claims.Id, dto);
    }

    public async Task RemoveTaskTag(JwtClaims claims, ChangeTaskTagRequestDto dto)
    {
        // Validate user exists
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
            
        await taskDataService.RemoveTaskTag(claims.Id, dto);
    }
}