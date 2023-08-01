using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(CancellationToken cancellationToken = default)
    {
        try
        {
            await _authService.Register(cancellationToken);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _authService.Login(cancellationToken));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}