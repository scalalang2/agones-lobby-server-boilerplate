using Microsoft.AspNetCore.Mvc;

namespace LobbyServer;

[ApiController]
[Route("api/hello")]
public class HelloServer : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Hello from the server!" });
    }
}