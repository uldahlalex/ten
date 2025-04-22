using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
public class MyControllerClass(MyDbContext ctx, ISecurityService securityService) : ControllerBase
{

    public const string GetDeviceLogsRoute = nameof(GetDeviceLogs);
    
    [HttpGet]
    [Route(nameof(GetDeviceLogsRoute))]
    public ActionResult<List<Devicelog>> GetDeviceLogs([FromHeader]string authorization)
    {
        var requester = securityService.VerifyJwtOrThrow(authorization);
        var result = ctx.Devicelogs.ToList();
        return Ok(result);
    }
}