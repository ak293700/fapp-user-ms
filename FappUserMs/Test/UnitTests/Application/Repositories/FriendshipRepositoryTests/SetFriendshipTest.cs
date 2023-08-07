using Application.Repositories;
using Domain.Entities.UserEntities;
using MongoDB.Driver;

namespace Test.UnitTests.Application.Repositories.FriendshipRepositoryTests;

public class SetFriendshipTest : BaseTest
{
    private readonly FriendshipRepository _friendshipRepository;

    public SetFriendshipTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        _friendshipRepository = new FriendshipRepository(Context);
    }

    [Fact]
    public async Task First_Request_Of_The_User_Should_Pass()
    {
        // Arrange
        const string userId = "64ce33852e7f31012418def1"; // Kevy
        const string friendId = "64ce33a62e7f31012418def2"; // Bret
        const JoiningState joiningState = JoiningState.Accepted;

        // Act
        await _friendshipRepository.SetFriendship(userId, friendId, joiningState);

        // Assert
        User? user = await Context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        Assert.NotNull(user);
        Assert.Single(user.Friends);
        Assert.Equal(friendId, user.Friends.First().UserId);
        Assert.Equal(joiningState, user.Friends.First().JoiningState);
    }

    [Fact]
    public async Task Not_First_Request_Of_The_User_Should_Pass()
    {
        // Arrange
        const string userId = "64ce32aeea1842cdf7250e88"; // Hugo
        const string friendId = "64ce33a62e7f31012418def2"; // Bret
        const JoiningState joiningState = JoiningState.AskedFromMe;

        // Act
        await _friendshipRepository.SetFriendship(userId, friendId, joiningState);

        // Assert
        User? user = await Context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        Assert.NotNull(user);

        Friend[] friends = user.Friends.ToArray();
        Assert.Equal(3, friends.Length);

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
        await _friendshipRepository.SetFriendship(userId, friendId, joiningState);
    }

    [Fact]
    public async Task Not_Existing_User2Id_Should_Add_Anyway()
    {
        // Arrange
        const string userId = "64ce32aeea1842cdf7250e88"; // Hugo
        const string friendId = "551132aeea1842cdf7250e00"; // Not existing
        const JoiningState joiningState = JoiningState.AskedFromMe;

        // Act
        await _friendshipRepository.SetFriendship(userId, friendId, joiningState);

        // Assert
        User? user = await Context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        Assert.NotNull(user);

        Friend[] friends = user.Friends.ToArray();
        Assert.Equal(3, friends.Length);

        Friend friend = friends.First(f => f.UserId == friendId);
        Assert.Equal(friendId, friend.UserId);
        Assert.Equal(joiningState, friend.JoiningState);
    }
}