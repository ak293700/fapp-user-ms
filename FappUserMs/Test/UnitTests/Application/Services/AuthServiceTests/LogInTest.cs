using Application.Common.Dtos.AuthDtos;
using Application.Services;
using FappCommon.Exceptions.ApplicationExceptions.UserExceptions;
using FappCommon.Mocks;

namespace Test.UnitTests.Application.Services.AuthServiceTests;

public class LogInTest : BaseTest
{
    private readonly AuthService _authService;

    public LogInTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        _authService = new AuthService(Context, ConfigurationMock.GetCustom(RegisterTest.MockAuthTokenConf));
    }

    [Fact]
    public async Task Existing_Should_Pass()
    {
        // Arrange
        LogInDto command = new LogInDto("alexandre.akdeniz@free.fr", "Ak2#2001");

        // Act
        string token = await _authService.LogIn(command);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Wrong_Email_Should_Throw()
    {
        // Arrange
        LogInDto command = new LogInDto("alexandre.akdeniz@wrong.email", "Ak2#2001");

        // Act + Assert
        await Assert.ThrowsAsync<UnauthorizedException>(
            async () => await _authService.LogIn(command)
        );
    }

    [Fact]
    public async Task Wrong_Password_Should_Throw()
    {
        // Arrange
        LogInDto command = new LogInDto("alexandre.akdeniz@free.fr", "WrongPassword#2001");

        // Act + Assert
        await Assert.ThrowsAsync<UnauthorizedException>(
            async () => await _authService.LogIn(command)
        );
    }

    [Fact]
    public async Task Email_And_Password_Exists_But_Wrong_Combination_Should_Throw()
    {
        // Arrange
        LogInDto command = new LogInDto("alexandre.akdeniz@free.fr", "Kevy#2002");

        // Act + Assert
        await Assert.ThrowsAsync<UnauthorizedException>(
            async () => await _authService.LogIn(command)
        );
    }
}