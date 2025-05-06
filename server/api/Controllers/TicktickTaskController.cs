using api.Models;
using api.Models.Dtos;
using api.Models.Dtos.Requests;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
public class TicktickTaskController(
    ISecurityService securityService,
    ITaskService taskService,
    ILogger<TicktickTaskController> logger) : Controller
{
    [HttpPost]
    [Route(nameof(GetMyTasks))]
    public async Task<ActionResult<List<TickticktaskDto>>> GetMyTasks(
        [FromHeader] string authorization,
        [FromBody] GetTasksFilterAndOrderParameters parameters)
    {
        var userClaims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.GetMyTasks(parameters, userClaims);
        return Ok(result);
    }

    [HttpPost]
    [Route(nameof(CreateTask))]
    public async Task<ActionResult<TickticktaskDto>> CreateTask(
        [FromBody] CreateTaskRequestDto dto,
        [FromHeader] string authorization)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.CreateTask(dto, claims);

        return Ok(result);
    }

    [HttpPatch]
    [Route(nameof(UpdateTask))]
    public async Task<ActionResult<TickticktaskDto>> UpdateTask(
        [FromBody] UpdateTaskRequestDto dto,
        [FromHeader] string authorization)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.UpdateTask(dto, claims);
        return Ok(result);
    }

    [HttpDelete]
    [Route(nameof(DeleteTask))]
    public async Task<ActionResult<TickticktaskDto>> DeleteTask(
        [FromHeader] string authorization, [FromQuery] string taskId)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        await taskService.DeleteTask(taskId, claims);
        return Ok();
    }

    [HttpGet]
    [Route(nameof(GetMyTags))]
    public async Task<ActionResult<List<TagDto>>> GetMyTags(
        [FromHeader] string authorization)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.GetMyTags(claims);
        return Ok(result);
    }

    [HttpGet]
    [Route(nameof(GetMyLists))]
    public async Task<ActionResult<List<TasklistDto>>> GetMyLists(
        [FromHeader] string authorization)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.GetMyLists(claims);
        return Ok(result);
    }

    [HttpPost]
    [Route(nameof(CreateList))]
    public async Task<ActionResult<TasklistDto>> CreateList([FromHeader] string authorization,
        [FromBody] CreateListRequestDto dto)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.CreateList(claims, dto);
        return Ok(result);
    }

    [HttpPost]
    [Route(nameof(CreateTag))]
    public async Task<ActionResult<TagDto>> CreateTag([FromHeader] string authorization,
        [FromBody] CreateTagRequestDto dto)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.CreateTag(claims, dto);
        return Ok(result);
    }

    [HttpPut]
    [Route(nameof(UpdateList))]
    public async Task<ActionResult<TasklistDto>> UpdateList([FromHeader] string authorization,
        [FromBody] UpdateListRequestDto dto)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.UpdateList(claims, dto);
        return Ok(result);
    }

    [HttpPut]
    [Route(nameof(UpdateTag))]
    public async Task<ActionResult<TagDto>> UpdateTag(
        [FromHeader] string authorization,
        [FromBody] UpdateTagRequestDto dto)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.UpdateTag(claims, dto);
        return Ok(result);
    }

    [HttpDelete]
    [Route(nameof(DeleteListWithTasks))]
    public async Task<ActionResult> DeleteListWithTasks(
        [FromHeader] string authorization, [FromQuery] string listId)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        await taskService.DeleteListWithAllTasks(listId, claims);
        return Ok();
    }

    [HttpDelete]
    [Route(nameof(DeleteTag))]
    public async Task<ActionResult> DeleteTag(
        [FromHeader] string authorization, [FromQuery] string tagId)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        await taskService.DeleteTag(tagId, claims);
        return Ok();
    }

    [HttpPut]
    [Route(nameof(AddTaskTag))]
    public async Task<ActionResult<TaskTagDto>> AddTaskTag([FromHeader] string authorization,
        [FromBody] ChangeTaskTagRequestDto dto)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.AddTagToTask(claims, dto);
        return Ok(result);
    }

    [HttpPut]
    [Route(nameof(RemoveTaskTag))]
    public async Task<ActionResult> RemoveTaskTag([FromHeader] string authorization,
        [FromBody] ChangeTaskTagRequestDto dto)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        await taskService.RemoveTaskTag(claims, dto);
        return Ok();
    }
}