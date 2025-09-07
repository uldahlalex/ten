using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;


[Controller()]
[ApiController]
[Route("api/[controller]/[action]")]
public class MyTestController : ControllerBase
{
    [HttpGet]
    public object Do([FromHeader]string authorization)
    {
        var u = JsonSerializer.Serialize(User, new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        });
        Console.WriteLine(u);
        return "Hello from MyTestController";
    }
}