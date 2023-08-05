namespace Application.Services;

public class FriendshipService
{
    private readonly FriendshipService _friendshipService;

    public FriendshipService(FriendshipService friendshipService)
    {
        _friendshipService = friendshipService;
    }
}