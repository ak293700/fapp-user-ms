using Application.Common.Dtos.UserDtos;
using Application.Services;
using FappCommon.CurrentUserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiController]
[Authorize]
public class FriendshipController : ControllerBase
{
    private readonly FriendshipService _friendshipService;
    private readonly ICurrentUserService _currentUserService;

    public FriendshipController(FriendshipService friendshipService, ICurrentUserService currentUserService)
    {
        _friendshipService = friendshipService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LiteUserDto>>> GetMyFriends(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _friendshipService.GetFriends(_currentUserService.UserId, cancellationToken));
        }
        catch (CustomException e)
        {
            return BadRequest(e.Message);
        }
    }
}