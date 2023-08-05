using Application.Common.Dtos.UserDtos;
using Domain.Entities.UserEntities;

namespace Application.Services;

public class FriendshipService
{
    private readonly IApplicationDbContext _context;

    public FriendshipService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LiteUserDto>> GetFriends(string userId, CancellationToken cancellationToken)
    {
        var friends = await _context.Users
            .Find(u => u.Id == userId)
            .Project(u => u.Friends
                .Where(f => f.JoiningState == JoiningState.Accepted)
                .Select(f => f.UserId)
            )
            .FirstOrDefaultAsync(cancellationToken);


        return Enumerable.Empty<LiteUserDto>();
    }
}