using Application.Repositories;
using Domain.Entities.UserEntities;

namespace Test.UnitTests.Application.Repositories.FriendshipRepositoryTests;

public class GetFriendshipTest : BaseTest
{
    private readonly FriendshipRepository _friendshipRepository;

    public GetFriendshipTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        _friendshipRepository = new FriendshipRepository(Context);
    }

    [Fact]
    public async Task From_Friends_Should_Pass()
    {
        // Arrange
        const string user1Id = "64ce33dd2e7f31012418def3"; // Ak2
        const string user2Id = "64ce33a62e7f31012418def2"; // Bret

        // Act
        JoiningState? fromSide1 = await _friendshipRepository.GetFriendship(user1Id, user2Id);
        JoiningState? fromSide2 = await _friendshipRepository.GetFriendship(user2Id, user1Id);

        // Assert
        Assert.Equal(JoiningState.Accepted, fromSide1);
        Assert.Equal(JoiningState.Accepted, fromSide2);
    }

    [Fact]
    public async Task Pending_Should_Pass()
    {
        // Arrange
        const string user1Id = "64ce32aeea1842cdf7250e88"; // Hugo
        const string user2Id = "64ce34002e7f31012418def5"; // Tim

        // Act
        JoiningState? fromSide1 = await _friendshipRepository.GetFriendship(user1Id, user2Id);
        JoiningState? fromSide2 = await _friendshipRepository.GetFriendship(user2Id, user1Id);

        // Assert
        Assert.Equal(JoiningState.AskedFromMe, fromSide1);
        Assert.Equal(JoiningState.AskedFromHim, fromSide2);
    }

    [Fact]
    public async Task Not_Friend_Should_Do()
    {
        // Arrange
        const string user1Id = "64ce32aeea1842cdf7250e88"; // Hugo
        const string user2Id = "64ce33852e7f31012418def1"; // Kevy

        // Act
        JoiningState? fromSide1 = await _friendshipRepository.GetFriendship(user1Id, user2Id);
        JoiningState? fromSide2 = await _friendshipRepository.GetFriendship(user2Id, user1Id);

        // Assert
        Assert.Null(fromSide1);
        Assert.Null(fromSide2);
    }
}