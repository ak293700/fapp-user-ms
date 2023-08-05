using Application.Common.Dtos.AuthDtos;
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
    public async Task<IActionResult> Register(RegisterDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            await _authService.Register(request, cancellationToken);
            return Ok();
        }
        catch (CustomException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LogInDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _authService.Login(request, cancellationToken));
        }
        catch (CustomException e)
        {
            return BadRequest(e.Message);
        }
    }
}