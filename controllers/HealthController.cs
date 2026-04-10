using Microsoft.AspNetCore.Mvc;

namespace xablau.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            message = "API rodando",
            project = "xablau"
        });
    }
}
