using Application.Common.Dtos.UserDtos;
using Application.Repositories;
using Domain.Entities.UserEntities;
using FappCommon.Exceptions.ApplicationExceptions.UserExceptions.Base;
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

    public async Task<IEnumerable<LiteUserDto>> GetUserFriends(string userId,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<string> friends = await _context.Users
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
            || !await _userRepository.Exist(friendId, cancellationToken))
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

    public async Task<IEnumerable<LiteUserDto>> GetUserPendingInvitations(string userId,
        CancellationToken cancellationToken = default)
    {
        // Check both users exist
        if (!await _userRepository.Exist(userId, cancellationToken))
            throw NotFoundDomainException.Instance;

        IEnumerable<string> friends = await _context.Users
            .Find(u => u.Id == userId)
            .Project(u => u.Friends
                .Where(f => f.JoiningState == JoiningState.AskedFromHim) // Only the ones that asked me
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

    /// <param name="userId">The one that will accept the friends requests</param>
    /// <param name="applicantId">The user that originally made the friend request</param>
    public async Task AcceptInvitation(string userId, string applicantId, CancellationToken cancellationToken = default)
    {
        // Check both users exist
        if (!await _userRepository.Exist(userId, cancellationToken)
            || !await _userRepository.Exist(applicantId, cancellationToken))
            throw NotFoundDomainException.Instance;

        JoiningState? joiningState =
            await _friendshipRepository.GetFriendship(userId, applicantId, cancellationToken);

        switch (joiningState)
        {
            case null:
                throw NotFoundDomainException.Instance;
            case JoiningState.Accepted:
                throw new AlreadyExistDomainException("Vous êtes déjà amis");
            case JoiningState.AskedFromMe:
                throw NotFoundDomainException.Instance; // There is no request from applicant
            case JoiningState.AskedFromHim:
            default:
                break; // Not to nest the code too much
        }

        // Accept the request
        await Task.WhenAll(
            _friendshipRepository.UpdateFriendship(userId, applicantId, JoiningState.Accepted, cancellationToken),
            _friendshipRepository.UpdateFriendship(applicantId, userId, JoiningState.Accepted, cancellationToken)
        );
    }

    public async Task DeclineInvitation(string userId, string applicantId,
        CancellationToken cancellationToken = default)
    {
        if (userId == applicantId)
            throw new UserException("Vous êtes déjà amis");

        // Check if the users exist
        if (!await _userRepository.Exist(userId, cancellationToken)
            || !await _userRepository.Exist(applicantId, cancellationToken))
            throw NotFoundDomainException.Instance;

        JoiningState? joiningState = await _friendshipRepository
            .GetFriendship(userId, applicantId, cancellationToken);

        switch (joiningState)
        {
            case null:
            case JoiningState.AskedFromMe:
                throw NotFoundDomainException.Instance; // There is no request from applicant
            case JoiningState.Accepted:
                throw new AlreadyExistDomainException("Vous êtes déjà amis");
            case JoiningState.AskedFromHim:
            default:
                break; // Not to nest the code too much
        }

        // Decline the request
        await _friendshipRepository.DeleteFriendshipBothSide(userId, applicantId, cancellationToken);
    }
}