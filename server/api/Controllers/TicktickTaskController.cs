using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
public class TicktickTaskController(MyDbContext ctx, ISecurityService securityService) : ControllerBase
{

    public const string GetDeviceLogsRoute = nameof(GetTasks);
    
    [HttpGet]
    [Route(nameof(GetDeviceLogsRoute))]
    public ActionResult<List<TickticktaskDto>> GetTasks([FromHeader]string authorization)
    {
        var requester = securityService.VerifyJwtOrThrow(authorization);
        var result = ctx.Tickticktasks.Select(task => new TickticktaskDto().FromEntity(task)).ToList();
        return Ok(result);
    }
}
