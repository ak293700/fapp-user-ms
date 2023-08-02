using Application.Common.Dtos.AuthDtos;
using Application.Common.Exceptions;
using Application.Services;
using Test.Mocks;

namespace Test.UnitTests;

public class FakeTest : BaseSeedTest
{
    private readonly AuthService _authService;

    private static readonly Dictionary<string, string?> MockConf = new Dictionary<string, string?>
    {
        { "Auth:Token", "aSeCrEtToKeN" }
    };

    public FakeTest()
    {
        _authService = new AuthService(Context, ConfigurationMock.GetCustom(MockConf));
    }

    [Fact]
    public async Task Should_Pass()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Zizou59", "zizou@marseille.msl", "ZiZou1998$");

        // Act
        await _authService.Register(command);
    }

    [Fact]
    public async Task Password_Has_no_Special_Character_Should_Throw()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Zizou59", "zizou@marseille.msl", "ZiZou1998");

        // Act + Assert
        await Assert.ThrowsAsync<DataValidationException>(async () => await _authService.Register(command));
    }
}