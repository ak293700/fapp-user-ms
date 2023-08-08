using Application.Repositories;
using Domain.Entities.UserEntities;
using MongoDB.Driver;

namespace Test.UnitTests.Application.Repositories.FriendshipRepositoryTests;

public class DeleteFriendshipBothSideTest : BaseTest
{
    private readonly FriendshipRepository _friendshipRepository;

    public DeleteFriendshipBothSideTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        _friendshipRepository = new FriendshipRepository(Context);
    }

    [Fact]
    public async Task Existing_Relationship()
    {
        // Arrange
        const string userId = "64ce33a62e7f31012418def2"; // Bret
        const string friendId = "64ce33dd2e7f31012418def3"; // Ak2

        // Act
        await _friendshipRepository.DeleteFriendshipBothSide(userId, friendId);

        // Assert
        User? user = await Context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        User? friend = await Context.Users.Find(u => u.Id == friendId).FirstOrDefaultAsync();

        Assert.NotNull(user);
        Assert.NotNull(friend);

        Assert.Empty(user.Friends);
        Assert.Equal(2, friend.Friends.Count);

        Assert.DoesNotContain(user.Friends, f => f.UserId == friendId);
        Assert.DoesNotContain(friend.Friends, f => f.UserId == userId);
    }

    [Fact]
    public async Task Not_Existing_Relationship_Do_Nothing()
    {
        // Arrange
        const string userId = "64ce33a62e7f31012418def2"; // Bret
        const string friendId = "64ce34002e7f31012418def5"; // Tim

        // Act
        await _friendshipRepository.DeleteFriendshipBothSide(userId, friendId);

        // Assert
        User? user = await Context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        User? friend = await Context.Users.Find(u => u.Id == friendId).FirstOrDefaultAsync();

        Assert.NotNull(user);
        Assert.NotNull(friend);

        Assert.Single(user.Friends);
        Assert.Equal(2, friend.Friends.Count);

        Assert.DoesNotContain(user.Friends, f => f.UserId == friendId);
        Assert.DoesNotContain(friend.Friends, f => f.UserId == userId);
    }

    [Fact]
    public async Task NotExisting_User_Should_Do_Nothing()
    {
        // Arrange
        const string userId = "55aa33a62e7f31012418dea8"; // Not existing
        const string friendId = "64ce33dd2e7f31012418def3"; // Ak2

        // Act
        await _friendshipRepository.DeleteFriendshipOneSide(userId, friendId);

        // Assert
        User? user = await Context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        User? friend = await Context.Users.Find(u => u.Id == friendId).FirstOrDefaultAsync();

        Assert.Null(user);
        Assert.NotNull(friend);

        Assert.Equal(3, friend.Friends.Count);
    }

    [Fact]
    public async Task NotExisting_Friend_Should_Do_Nothing()
    {
        // Arrange
        const string userId = "64ce33a62e7f31012418def2"; // Bret
        const string friendId = FakeId; // Not existing

        // Act
        await _friendshipRepository.DeleteFriendshipOneSide(userId, friendId);

        // Assert
        User? user = await Context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        User? friend = await Context.Users.Find(u => u.Id == friendId).FirstOrDefaultAsync();

        Assert.NotNull(user);
        Assert.Null(friend);

        Assert.Single(user.Friends);
    }
}