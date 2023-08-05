using Application.Common.Dtos.UserDtos;
using Application.Services;
using FappCommon.CurrentUserService;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiController]
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
    public async Task<ActionResult<LiteUserDto>> GetMyFriends(CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok();
        }
        catch (CustomException e)
        {
            return BadRequest(e.Message);
        }
    }
}