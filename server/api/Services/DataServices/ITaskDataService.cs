using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;

namespace api.Services;

public interface ITaskDataService
{
    public Task<List<TickticktaskDto>> GetMyTasks(GetTasksFilterAndOrderParameters parameters, string userId);
    public Task<TickticktaskDto> CreateTask(CreateTaskRequestDto dto, string userId);
    Task<TickticktaskDto> UpdateTask(UpdateTaskRequestDto dto, string userId);
    Task DeleteTask(string taskId, string userId);
    Task<List<TasklistDto>> GetMyLists(string userId);
    Task<List<TagDto>> GetMyTags(string userId);
    Task<TasklistDto> CreateList(string userId, CreateListRequestDto dto);
    Task<TagDto> CreateTag(string userId, CreateTagRequestDto dto);
    Task<TasklistDto> UpdateList(string userId, UpdateListRequestDto dto);
    Task<TagDto> UpdateTag(string userId, UpdateTagRequestDto dto);
    Task DeleteListWithAllTasks(string listId, string userId);
    Task DeleteTag(string tagId, string userId);
    Task<TaskTagDto> AddTagToTask(string userId, ChangeTaskTagRequestDto dto);
    Task RemoveTaskTag(string userId, ChangeTaskTagRequestDto dto);
}