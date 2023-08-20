using Application.Common.Dtos.UserDtos;
using Application.Services;
using FappCommon.Exceptions.DomainExceptions;
using FappCommon.Interfaces.ICurrentUserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("/user/friend")]
[Consumes("application/json")]
[Produces("application/json")]
[ApiController]
[Authorize]
public class FriendshipController : ControllerBase
{
    private readonly FriendshipService _friendshipService;
    private readonly ICurrentUserServiceString _currentUserService;

    public FriendshipController(FriendshipService friendshipService, ICurrentUserServiceString currentUserService)
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
            return Ok(await _friendshipService.GetUserFriends(_currentUserService.UserId, cancellationToken));
        }
        catch (NotFoundDomainException e)
        {
            return NotFound(e.Message);
        }
        catch (CustomException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<LiteUserDto>>> GetMyPendingInvitations(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _friendshipService.GetUserPendingInvitations(_currentUserService.UserId,
                cancellationToken));
        }
        catch (NotFoundDomainException e)
        {
            return NotFound(e.Message);
        }
        catch (CustomException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("invite/{friendId}")]
    public async Task<IActionResult> Invite(string friendId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _friendshipService.Invite(_currentUserService.UserId, friendId, cancellationToken);
            return Ok();
        }
        catch (NotFoundDomainException e)
        {
            return NotFound(e.Message);
        }
        catch (CustomException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("accept/{applicantId}")]
    public async Task<IActionResult> AcceptInvitation(string applicantId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _friendshipService.AcceptInvitation(_currentUserService.UserId, applicantId, cancellationToken);
            return Ok();
        }
        catch (NotFoundDomainException e)
        {
            return NotFound(e.Message);
        }
        catch (CustomException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("decline/{applicantId}")]
    public async Task<IActionResult> DeclineInvitation(string applicantId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _friendshipService.DeclineInvitation(_currentUserService.UserId, applicantId, cancellationToken);
            return Ok();
        }
        catch (NotFoundDomainException e)
        {
            return NotFound(e.Message);
        }
        catch (CustomException e)
        {
            return BadRequest(e.Message);
        }
    }

    // [HttpDelete("{friendId:guid}")]
    // public async Task<IActionResult> RemoveFriend(Guid friendId, CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         await _friendshipService.RemoveFriend(_currentUserService.UserId, friendId, cancellationToken);
    //         return NoContent();
    //     }
    //     catch (NotFoundDomainException e)
    //     {
    //         return NotFound(e.Message);
    //     }
    //     catch (CustomException e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }
}