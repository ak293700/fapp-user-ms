using Application.Common.Dtos.UserDtos;
using Application.Repositories;
using Domain.Entities.UserEntities;
using FappCommon.Exceptions.Base;
using FappCommon.Exceptions.DomainExceptions;

namespace Application.Services;

public class FriendshipService
{
    private readonly IApplicationDbContext _context;
    private readonly FriendshipRepository _friendshipRepository;
    private readonly UserRepository _userRepository;

    public FriendshipService(IApplicationDbContext context, FriendshipRepository friendshipRepository,
        UserRepository userRepository)
    {
        _context = context;
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<LiteUserDto>> GetUserFriends(string userId, CancellationToken cancellationToken)
    {
        var friends = await _context.Users
            .Find(u => u.Id == userId)
            .Project(u => u.Friends
                .Where(f => f.JoiningState == JoiningState.Accepted)
                .Select(f => f.UserId)
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (friends == null)
            throw NotFoundDomainException.Instance;

        return await _context.Users
            .Find(u => friends.Contains(u.Id))
            .Project(u => new LiteUserDto(u.Id, u.UserName))
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task Invite(string applicantId, string friendId, CancellationToken cancellationToken = default)
    {
        if (applicantId == friendId)
            throw new CustomException("Vous ne pouvez pas vous ajouter vous-même en ami");

        // Check both users exist
        if (!await _userRepository.Exist(applicantId, cancellationToken)
            || !await _userRepository.Exist(applicantId, cancellationToken))
            throw NotFoundDomainException.Instance;

        JoiningState?[] joiningStates = await Task.WhenAll(_friendshipRepository
                .GetFriendship(applicantId, friendId, cancellationToken),
            _friendshipRepository
                .GetFriendship(friendId, applicantId, cancellationToken));

        JoiningState? applicantSideState = joiningStates[0];
        JoiningState? friendSideState = joiningStates[1];

        if (applicantSideState == JoiningState.Accepted || friendSideState == JoiningState.Accepted)
            throw new AlreadyExistDomainException("Vous êtes déjà amis");

        if (applicantSideState == JoiningState.Pending)
            throw new AlreadyExistDomainException("Une demande d'ami est déjà en cours");

        if (applicantSideState == JoiningState.Pending)
        {
            // Accept the existing invitation
            await _friendshipRepository.SetFriendship(applicantId, friendId, JoiningState.Accepted, cancellationToken);
            await _friendshipRepository.UpdateFriendship(friendId, applicantId, JoiningState.Accepted,
                cancellationToken);
            return;
        }

        // Send the invitation
        await _friendshipRepository.SetFriendship(applicantId, friendId, JoiningState.Pending, cancellationToken);
    }
}