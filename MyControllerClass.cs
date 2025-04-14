using Microsoft.AspNetCore.Mvc;

namespace ten;

[ApiController]
public class MyControllerClass : ControllerBase
{
    [HttpGet]
    [Route(nameof(MyEndpoint))]
    public ActionResult MyEndpoint()
    {
        return Ok();
    }
}