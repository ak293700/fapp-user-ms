using Application.Repositories;

namespace Test.UnitTests.Application.Repositories.UserRepositoryTests;

public class ExistsTest : BaseTest
{
    private readonly UserRepository _userRepository;

    public ExistsTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        _userRepository = new UserRepository(Context);
    }

    [Fact]
    public async Task Exists_Should_Return_True()
    {
        // Arrange
        const string userId = "64ce33e82e7f31012418def4"; // Enzo

        // Act
        bool exists = await _userRepository.Exist(userId);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task Dont_Exists_Should_Return_True()
    {
        // Arrange
        const string userId = "55aa33e82e7f31012418dee8"; // Enzo

        // Act
        bool exists = await _userRepository.Exist(userId);

        // Assert
        Assert.False(exists);
    }
}