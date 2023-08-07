using Application.Repositories;
using Application.Services;
using Domain.Entities.UserEntities;
using FappCommon.Exceptions.DomainExceptions;
using MongoDB.Driver;

namespace Test.UnitTests.Application.Services.FriendshipServiceTests;

public class AcceptInvitationTest : BaseTest
{
    private readonly FriendshipService _friendshipService;

    public AcceptInvitationTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        _friendshipService =
            new FriendshipService(Context, new FriendshipRepository(Context), new UserRepository(Context));
    }

    [Fact]
    // public async Task NoFriendship_Should_Throw()
    public async Task NoFriendship_Should_Throw()
    {
        // Arrange
        const string applicantId = "64ce33852e7f31012418def1"; // Kevy
        const string friendId = "64ce33a62e7f31012418def2"; // Bret

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundDomainException>(
            async () => await _friendshipService.AcceptInvitation(applicantId, friendId)
        );
    }

    [Fact]
    public async Task AskedFromMe_Should_Throw()
    {
        // Arrange
        const string askedFromId = "64ce32aeea1842cdf7250e88"; // Hugo
        const string askedToId = "64ce34002e7f31012418def5"; // Tim

        // Act + Assert

        await Assert.ThrowsAsync<NotFoundDomainException>( // Reverse
            async () => await _friendshipService.AcceptInvitation(askedFromId, askedToId)
        );
    }

    [Fact]
    public async Task AskedFromHim_Should_Pass()
    {
        // Arrange
        const string askedToId = "64ce34002e7f31012418def5"; // Tim
        const string askedFromId = "64ce32aeea1842cdf7250e88"; // Hugo

        // Act
        await _friendshipService.AcceptInvitation(askedToId, askedFromId);

        // Assert
        User? hugo = await Context.Users.Find(u => u.Id == askedFromId).FirstOrDefaultAsync();
        User? tim = await Context.Users.Find(u => u.Id == askedToId).FirstOrDefaultAsync();
        Assert.NotNull(hugo);
        Assert.NotNull(tim);

        Friend[] hugoFriends = hugo.Friends.ToArray();
        Friend[] timFriends = tim.Friends.ToArray();
        Assert.Equal(2, hugoFriends.Length);
        Assert.Equal(2, timFriends.Length);

        Friend hugoFromTim = hugoFriends.First(f => f.UserId == askedToId);
        Friend timFromHugo = timFriends.First(f => f.UserId == askedFromId);
        Assert.Equal(JoiningState.Accepted, timFromHugo.JoiningState);
        Assert.Equal(JoiningState.Accepted, hugoFromTim.JoiningState);
    }

    [Fact]
    public async Task NotExisting_Applicant_Should_Throw()
    {
        // Arrange
        const string applicantId = "55aa34002e7f31012418deb4"; // Not existing
        const string friendId = "64ce32aeea1842cdf7250e88"; // Hugo

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundDomainException>(async () =>
            await _friendshipService.Invite(applicantId, friendId)
        );
    }

    [Fact]
    public async Task NotExisting_Friend_Should_Throw()
    {
        // Arrange
        const string applicantId = "64ce32aeea1842cdf7250e88"; // Hugo
        const string friendId = "55aa34002e7f31012418deb4"; // NotExisting

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundDomainException>(async () =>
            await _friendshipService.Invite(applicantId, friendId)
        );
    }

    [Fact]
    public async Task Already_Friends_Should_Throw()
    {
        // Arrange
        const string user1Id = "64ce33a62e7f31012418def2"; // Bret
        const string user2Id = "64ce33dd2e7f31012418def3"; // Ak2

        // Act + Assert
        await Assert.ThrowsAsync<AlreadyExistDomainException>(
            async () => await _friendshipService.AcceptInvitation(user1Id, user2Id)
        );
    }
}