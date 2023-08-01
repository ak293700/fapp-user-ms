using Application.Common.Dtos.UserDtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _userService.GetAll(cancellationToken));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}