using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("/user/friend")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiController]
public class FriendshipController : ControllerBase
{
    private readonly FriendshipService _friendshipService;

    public FriendshipController(FriendshipService friendshipService)
    {
        _friendshipService = friendshipService;
    }
}