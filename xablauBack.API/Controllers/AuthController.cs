using Microsoft.AspNetCore.Mvc;
using xablauBack.Application.Contracts.Auth; 

namespace xablauBack.API.Controllers;

[ApiController]
[Route("api/[controller]")]

/* base para controllers */
public class AuthController : ControllerBase
{
    /* logica de cadastro */
    private readonly IAuthService _authService;

    /* dependencia injetada */
    public AuthController(IAuthService authService)
    { 
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }
}

