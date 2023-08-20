using Application.Common.Dtos.UserDtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiController]
// [Authorize]
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
        try
        {
            return Ok(await _userService.GetAll(cancellationToken));
        }
        catch (CustomException e)
        {
            return BadRequest(e.Message);
        }
    }
}