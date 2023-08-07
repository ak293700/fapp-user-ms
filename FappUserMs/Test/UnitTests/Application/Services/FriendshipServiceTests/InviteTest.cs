using Application.Repositories;
using Application.Services;
using Domain.Entities.UserEntities;
using FappCommon.Exceptions.DomainExceptions;
using MongoDB.Driver;

namespace Test.UnitTests.Application.Services.FriendshipServiceTests;

public class InviteTest : BaseTest
{
    private readonly FriendshipService _friendshipService;

    public InviteTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        _friendshipService =
            new FriendshipService(Context, new FriendshipRepository(Context), new UserRepository(Context));
    }

    [Fact]
    public async Task NoFriendship_Should_Pass()
    {
        // Arrange
        const string applicantId = "64ce33852e7f31012418def1"; // Kevy
        const string friendId = "64ce33a62e7f31012418def2"; // Bret

        // Act
        await _friendshipService.Invite(applicantId, friendId);

        // Assert
        User? user = await Context.Users.Find(u => u.Id == applicantId).FirstOrDefaultAsync();
        Assert.NotNull(user);

        Friend[] friends = user.Friends.ToArray();
        Assert.Single(friends);

        Assert.Equal(friendId, friends[0].UserId);
    }

    [Fact]
    public async Task Already_AskedFromMe_Should_Throw()
    {
        // Arrange
        const string applicantId = "64ce32aeea1842cdf7250e88"; // Hugo
        const string friendId = "64ce34002e7f31012418def5"; // Tim

        // Act + Assert

        await Assert.ThrowsAsync<AlreadyExistDomainException>(
            async () => await _friendshipService.Invite(applicantId, friendId)
        );
    }

    [Fact]
    public async Task Already_AskedFromHin_Should_Pass()
    {
        // Arrange
        const string applicantId = "64ce34002e7f31012418def5"; // Tim
        const string friendId = "64ce32aeea1842cdf7250e88"; // Hugo

        // Act
        await _friendshipService.Invite(applicantId, friendId);

        // Assert
        User? applicant = await Context.Users.Find(u => u.Id == applicantId).FirstOrDefaultAsync();
        User? friend = await Context.Users.Find(u => u.Id == friendId).FirstOrDefaultAsync();
        Assert.NotNull(applicant);
        Assert.NotNull(friend);

        Friend[] applicantFriends = applicant.Friends.ToArray();
        Friend[] friendFriends = friend.Friends.ToArray();
        Assert.Equal(2, applicantFriends.Length);
        Assert.Equal(2, friendFriends.Length);

        Friend hugoFromTim = applicantFriends.First(f => f.UserId == friendId);
        Friend timFromHugo = friendFriends.First(f => f.UserId == applicantId);
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
}