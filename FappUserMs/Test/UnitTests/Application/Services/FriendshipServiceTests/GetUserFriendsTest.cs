using Application.Repositories;
using Application.Services;
using FappCommon.Exceptions.DomainExceptions;

namespace Test.UnitTests.Application.Services.FriendshipServiceTests;

public class GetUserFriendsTest : BaseTest
{
    private readonly FriendshipService _friendshipService;

    public GetUserFriendsTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        _friendshipService = new FriendshipService(Context,
            new FriendshipRepository(Context),
            new UserRepository(Context));
    }

    [Theory]
    [InlineData("64ce33dd2e7f31012418def3", 3)] // Ak2
    [InlineData("64ce32aeea1842cdf7250e88", 1)] // Hugo
    [InlineData("64ce34002e7f31012418def5", 1)] // Tim
    [InlineData("64ce33852e7f31012418def1", 0)] // Kevy
    public async Task Existing_Should_Pass(string userId, int expectedCount)
    {
        // Act
        var friends = await _friendshipService.GetUserFriends(userId);

        // Assert
        Assert.Equal(expectedCount, friends.Count());
    }

    [Fact]
    public async Task NotExisting_Should_Throw()
    {
        // Arrange
        const string userId = "54ce33852e7f31412418de91";

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundDomainException>(async () =>
            await _friendshipService.GetUserFriends(userId)
        );
    }
}