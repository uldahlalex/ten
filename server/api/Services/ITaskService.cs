using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;

namespace api.Services;

public interface ITaskService
{
    public Task<List<TickticktaskDto>> GetMyTasks(TaskFilteringAndSorting parameters, string requesterId);
    public Task<TickticktaskDto> CreateTask(CreateTaskRequestDto dto, string requesterId);
    Task<TickticktaskDto> UpdateTask(UpdateTaskRequestDto dto, string requesterId);
    Task DeleteTask(string taskId, string requesterId);
    Task<List<TasklistDto>> GetMyLists(string requesterId);
    Task<List<TagDto>> GetMyTags(string requesterId);
    Task<TasklistDto> CreateList(string requesterId, CreateListRequestDto dto);
    Task<TagDto> CreateTag(string requesterId, CreateTagRequestDto dto);
    Task<TasklistDto> UpdateList(string requesterId, UpdateListRequestDto dto);
    Task<TagDto> UpdateTag(string requesterId, UpdateTagRequestDto dto);

    Task DeleteListWithAllTasks(string listId, string requesterId);
    Task DeleteTag(string tagId, string requesterId);
    Task<TaskTagDto> AddTagToTask(string requesterId, ChangeTaskTagRequestDto dto);
    Task RemoveTaskTag(string requesterId, ChangeTaskTagRequestDto dto);
}