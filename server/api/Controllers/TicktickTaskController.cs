using efscaffold.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
public class TicktickTaskController(MyDbContext ctx, ISecurityService securityService) : ControllerBase
{

    public const string GetDeviceLogsRoute = nameof(GetDeviceLogs);
    
    [HttpGet]
    [Route(nameof(GetDeviceLogsRoute))]
    public ActionResult<List<Tickticktask>> GetDeviceLogs([FromHeader]string authorization)
    {
        var requester = securityService.VerifyJwtOrThrow(authorization);
        var result = ctx.Tickticktasks.ToList();
        return Ok(result);
    }
}