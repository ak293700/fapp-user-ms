using Application.Repositories;
using Domain.Entities.UserEntities;
using MongoDB.Driver;

namespace Test.UnitTests.Application.Repositories.FriendshipRepositoryTests;

public class UpdateFriendshipTest : BaseTest
{
    private readonly FriendshipRepository _friendshipRepository;

    public UpdateFriendshipTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        _friendshipRepository = new FriendshipRepository(Context);
    }

    [Fact]
    public async Task Not_Existing_Friendship_Should_Do_Nothing()
    {
        // Arrange
        const string userId = "64ce33852e7f31012418def1"; // Kevy
        const string friendId = "64ce33a62e7f31012418def2"; // Bret
        const JoiningState joiningState = JoiningState.Accepted;

        // Act
        await _friendshipRepository.UpdateFriendship(userId, friendId, joiningState);

        // Assert
        User? user = await Context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        Assert.NotNull(user);
        Assert.Empty(user.Friends);
    }

    [Fact]
    public async Task Existing_Friendship_Should_Pass()
    {
        // Arrange
        const string userId = "64ce34002e7f31012418def5"; // Tim
        const string friendId = "64ce32aeea1842cdf7250e88"; // Hugo
        const JoiningState joiningState = JoiningState.Accepted;

        // Act
        await _friendshipRepository.UpdateFriendship(userId, friendId, joiningState);

        // Assert
        User? user = await Context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        Assert.NotNull(user);

        Friend[] friends = user.Friends.ToArray();
        Assert.Equal(2, friends.Length);

        Friend friend = friends.First(f => f.UserId == friendId);
        Assert.Equal(friendId, friend.UserId);
        Assert.Equal(joiningState, friend.JoiningState);
    }

    [Fact]
    public async Task Not_Existing_User1Id_Should_Do_Nothing()
    {
        // Arrange
        const string userId = "551132aeea1842cdf7250e00"; // Not existing
        const string friendId = "64ce33a62e7f31012418def2"; // Bret
        const JoiningState joiningState = JoiningState.AskedFromMe;

        // Act
        await _friendshipRepository.UpdateFriendship(userId, friendId, joiningState);
    }

    [Fact]
    public async Task Not_Existing_User2Id_Should_Do_Nothing()
    {
        // Arrange
        const string userId = "64ce32aeea1842cdf7250e88"; // Hugo
        const string friendId = "551132aeea1842cdf7250e00"; // Not existing
        const JoiningState joiningState = JoiningState.AskedFromMe;

        // Act
        await _friendshipRepository.UpdateFriendship(userId, friendId, joiningState);
    }
}