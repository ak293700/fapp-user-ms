using Application.Common.Dtos.UserDtos;
using Application.Repositories;
using Application.Services;

namespace Test.UnitTests.Application.Services.FriendshipServiceTests;

public class GetUserPendingInvitationsTest : BaseTest
{
    private readonly FriendshipService _friendshipService;

    public GetUserPendingInvitationsTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        _friendshipService =
            new FriendshipService(Context, new FriendshipRepository(Context), new UserRepository(Context));
    }

    [Fact]
    public async Task One_Request_Should_Pass()
    {
        // Arrange
        const string userId = "64ce34002e7f31012418def5"; // Tim

        // Act
        IEnumerable<LiteUserDto> result = await _friendshipService.GetUserPendingInvitations(userId);

        // Assert
        LiteUserDto[] pendings = result as LiteUserDto[] ?? result.ToArray();
        Assert.Single(pendings);

        const string hugoId = "64ce32aeea1842cdf7250e88";
        Assert.Equal(hugoId, pendings.First().Id);
    }

    [Fact]
    public async Task No_Request_Should_Be_Empty()
    {
        // Arrange
        const string userId = "64ce33e82e7f31012418def4"; // Enzo

        // Act
        IEnumerable<LiteUserDto> result = await _friendshipService.GetUserPendingInvitations(userId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task No_Request_But_Request_Someone_Itself_Should_Pass()
    {
        // Arrange
        const string userId = "64ce32aeea1842cdf7250e88"; // Hugo

        // Act
        IEnumerable<LiteUserDto> result = await _friendshipService.GetUserPendingInvitations(userId);

        // Assert
        Assert.Empty(result);
    }
}