using Application.Common.Dtos.UserDtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("/user")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LiteUserDto>>> GetAll(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("GetAll");
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