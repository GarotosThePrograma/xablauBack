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
        
        if (!result.Sucesso)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")] /* cria a rota "POST /api/auth/login" */
    public async Task<IActionResult> Login([FromBody] LoginRequest request) /* [FromBody] transforma o JSON do corpo da req em um onj C# */
    {
        var result = await _authService.LoginAsync(request); /* chama a regra de negocio do login */

        if (!result.Sucesso)
        {
            return BadRequest(result); /* retorna um 400 se a senha ou email estiverem errados */
        }

        return Ok(result); /* se tiver certo retorna 200 */
    }
}

