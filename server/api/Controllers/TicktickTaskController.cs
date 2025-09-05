using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class TicktickTaskController(
    IJwtService jwtService,
    IUserDataService userDataService,
    ITaskService taskService,
    ILogger<TicktickTaskController> logger) : Controller
{
    [HttpPost]
    [Route(nameof(GetMyTasks))]
    public async Task<ActionResult<List<TickticktaskDto>>> GetMyTasks(
        [FromBody] MyAmazingFilteringStuff parameters)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var jwtClaims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(jwtClaims.Id))
            throw new Exception("User does not exist");
        var result = await taskService.GetMyTasks(parameters, jwtClaims);
        return result;
    }

    [HttpPost]
    [Route(nameof(CreateTask))]
    public async Task<ActionResult<TickticktaskDto>> CreateTask(
        [FromBody] CreateTaskRequestDto dto)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        var result = await taskService.CreateTask(dto, claims);

        return result;
    }

    [HttpPatch]
    [Route(nameof(UpdateTask))]
    public async Task<ActionResult<TickticktaskDto>> UpdateTask(
        [FromBody] UpdateTaskRequestDto dto)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        var result = await taskService.UpdateTask(dto, claims);
        return result;
    }

    [HttpDelete]
    [Route(nameof(DeleteTask))]
    public async Task<ActionResult<TickticktaskDto>> DeleteTask(
        [FromQuery] string taskId)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        await taskService.DeleteTask(taskId, claims);
        return Ok();
    }

    [HttpGet]
    [Route(nameof(GetMyTags))]
    public async Task<ActionResult<List<TagDto>>> GetMyTags()
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        var result = await taskService.GetMyTags(claims);
        return result;
    }

    [HttpGet]
    [Route(nameof(GetMyLists))]
    public async Task<ActionResult<List<TasklistDto>>> GetMyLists()
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        var result = await taskService.GetMyLists(claims);
        return result;
    }

    [HttpPost]
    [Route(nameof(CreateList))]
    public async Task<ActionResult<TasklistDto>> CreateList(
        [FromBody] CreateListRequestDto dto)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        var result = await taskService.CreateList(claims, dto);
        return result;
    }

    [HttpPost]
    [Route(nameof(CreateTag))]
    public async Task<ActionResult<TagDto>> CreateTag(
        [FromBody] CreateTagRequestDto dto)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        var result = await taskService.CreateTag(claims, dto);
        return result;
    }

    [HttpPut]
    [Route(nameof(UpdateList))]
    public async Task<ActionResult<TasklistDto>> UpdateList(
        [FromBody] UpdateListRequestDto dto)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        var result = await taskService.UpdateList(claims, dto);
        return result;
    }

    [HttpPut]
    [Route(nameof(UpdateTag))]
    public async Task<ActionResult<TagDto>> UpdateTag(
        [FromBody] UpdateTagRequestDto dto)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        var result = await taskService.UpdateTag(claims, dto);
        return result;
    }

    [HttpDelete]
    [Route(nameof(DeleteListWithTasks))]
    public async Task<ActionResult> DeleteListWithTasks(
        [FromQuery] string listId)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        await taskService.DeleteListWithAllTasks(listId, claims);
        return Ok();
    }

    [HttpDelete]
    [Route(nameof(DeleteTag))]
    public async Task<ActionResult> DeleteTag(
        [FromQuery] string tagId)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        await taskService.DeleteTag(tagId, claims);
        return Ok();
    }

    [HttpPut]
    [Route(nameof(AddTaskTag))]
    public async Task<ActionResult<TaskTagDto>> AddTaskTag(
        [FromBody] ChangeTaskTagRequestDto dto)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        var result = await taskService.AddTagToTask(claims, dto);
        return result;
    }

    [HttpPut]
    [Route(nameof(RemoveTaskTag))]
    public async Task<ActionResult> RemoveTaskTag(
        [FromBody] ChangeTaskTagRequestDto dto)
    {
        var authorization = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var claims = jwtService.VerifyJwt(authorization);
        if (!await userDataService.UserExistsAsync(claims.Id))
            throw new Exception("User does not exist");
        await taskService.RemoveTaskTag(claims, dto);
        return Ok();
    }
}