using api.Models;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;

namespace api.Services;

public interface ITaskService
{
    public Task<List<TickticktaskDto>> GetMyTasks(MyFiltering parameters, JwtClaims jwtClaims);
    public Task<TickticktaskDto> CreateTask(CreateTaskRequestDto dto, JwtClaims claims);
    Task<TickticktaskDto> UpdateTask(UpdateTaskRequestDto dto, JwtClaims claims);
    Task DeleteTask(string taskId, JwtClaims claims);
    Task<List<TasklistDto>> GetMyLists(JwtClaims claims);
    Task<List<TagDto>> GetMyTags(JwtClaims claims);
    Task<TasklistDto> CreateList(JwtClaims claims, CreateListRequestDto dto);
    Task<TagDto> CreateTag(JwtClaims claims, CreateTagRequestDto dto);
    Task<TasklistDto> UpdateList(JwtClaims claims, UpdateListRequestDto dto);
    Task<TagDto> UpdateTag(JwtClaims claims, UpdateTagRequestDto dto);

    Task DeleteListWithAllTasks(string listId, JwtClaims claims);
    Task DeleteTag(string tagId, JwtClaims claims);
    Task<TaskTagDto> AddTagToTask(JwtClaims claims, ChangeTaskTagRequestDto dto);
    Task RemoveTaskTag(JwtClaims claims, ChangeTaskTagRequestDto dto);
}