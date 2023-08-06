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
        // Todo there is a bug here
        var friends = await _context.Users
            .Find(u => u.Id == userId)
            .Project(u => u.Friends
                .Where(f => f.JoiningState == JoiningState.Accepted)
                .Select(f => f.UserId)
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (friends == null)
            throw NotFoundDomainException.Instance;

        var res = await _context.Users
            .Find(u => friends.Contains(u.Id))
            .Project(u => new LiteUserDto(u.Id, u.UserName))
            .ToListAsync(cancellationToken: cancellationToken);

        return res;
    }

    public async Task Invite(string applicantId, string friendId, CancellationToken cancellationToken = default)
    {
        if (applicantId == friendId)
            throw new CustomException("Vous ne pouvez pas vous ajouter vous-même en ami");

        // Check both users exist
        if (!await _userRepository.Exist(applicantId, cancellationToken)
            || !await _userRepository.Exist(applicantId, cancellationToken))
            throw NotFoundDomainException.Instance;

        JoiningState? applicantSideState = await _friendshipRepository.GetFriendship(applicantId, friendId,
            cancellationToken);

        switch (applicantSideState)
        {
            case JoiningState.Accepted:
                throw new AlreadyExistDomainException("Vous êtes déjà amis");
            case JoiningState.AskedFromMe:
                throw new AlreadyExistDomainException("Une demande d'ami est déjà en cours");
            case JoiningState.AskedFromHim: // Accept the existing invitation
            {
                await Task.WhenAll(
                    _friendshipRepository.UpdateFriendship(applicantId, friendId, JoiningState.Accepted,
                        cancellationToken),
                    _friendshipRepository.UpdateFriendship(friendId, applicantId, JoiningState.Accepted,
                        cancellationToken)
                );
                return;
            }
            default:
                break; // Not to nest the code too much
        }

        // Create the request
        await _friendshipRepository.SetFriendship(applicantId, friendId, JoiningState.AskedFromMe, cancellationToken);
        await _friendshipRepository.SetFriendship(friendId, applicantId, JoiningState.AskedFromHim, cancellationToken);
    }
}