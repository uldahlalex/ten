using efscaffold.Entities;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
public class TicktickTaskController( 
    ISecurityService securityService,
    ITaskService taskService,
    ILogger<TicktickTaskController> logger) : Controller
{

    
    public const string GetTasksRoute = nameof(GetTasks);

    public const string CreateTaskRoute = nameof(CreateTask);
    public const string UpdateTaskRoute = nameof(UpdateTask);

    [HttpPost]
    [Route(GetTasksRoute)]
    public async Task<ActionResult<List<TickticktaskDto>>> GetTasks(
        [FromHeader] string authorization,
        [FromBody]GetTasksFilterAndOrderParameters parameters)
    {
        var userClaims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.GetTasks(parameters, userClaims);
        return Ok(result);
    }
    
    [HttpPost]
    [Route(CreateTaskRoute)]
    public async Task<ActionResult<TickticktaskDto>> CreateTask(
        [FromBody] CreateTaskRequestDto dto,
        [FromHeader] string authorization)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.CreateTask(dto, claims);
        return Ok(result);
    }
    
    [HttpPatch]
    [Route(CreateTaskRoute)]
    public async Task<ActionResult<TickticktaskDto>> UpdateTask(
        [FromBody] UpdateTaskRequestDto dto,
        [FromHeader] string authorization)
    {
        var claims = securityService.VerifyJwtOrThrow(authorization);
        var result = await taskService.UpdateTask(dto, claims);
        return Ok(result);
    }

    

}