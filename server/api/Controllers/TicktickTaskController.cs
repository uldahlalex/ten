using System.Text.Json;
using efscaffold.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api;

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
}