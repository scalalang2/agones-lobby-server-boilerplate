using Microsoft.AspNetCore.Mvc;

namespace LobbyServer;

[ApiController]
[Route("api/game")]
public class ApiServer(GameServerAllocator allocator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var allocation = await allocator.AllocateAsync();
            return Ok(allocation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
