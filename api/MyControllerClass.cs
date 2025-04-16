using Core.Domain.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;

namespace ten;

[ApiController]
public class MyControllerClass(MyDbContext ctx) : ControllerBase
{

    public const string GetDeviceLogsRoute = nameof(GetDeviceLogs);
    
    [HttpGet]
    [Route(nameof(GetDeviceLogsRoute))]
    public ActionResult<List<Devicelog>> GetDeviceLogs()
    {
        var result = ctx.Devicelogs.ToList();
        return Ok(result);
    }
}