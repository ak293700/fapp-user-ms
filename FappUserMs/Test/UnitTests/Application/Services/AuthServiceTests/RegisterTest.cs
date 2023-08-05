using Application.Common.Dtos.AuthDtos;
using Application.Services;
using FappCommon.Exceptions.ApplicationExceptions;
using FappCommon.Exceptions.DomainExceptions;
using FappCommon.Mocks;

namespace Test.UnitTests.Application.Services.AuthServiceTests;

public class RegisterTest : BaseTest
{
    private readonly AuthService _authService;

    public static Dictionary<string, string?> MockAuthTokenConf { get; } =
        new() { { "Auth:Token", "0gboa*&(v-#hncIBO87OF" } };

    public RegisterTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        _authService = new AuthService(Context, ConfigurationMock.GetCustom(MockAuthTokenConf));
    }

    [Fact]
    public async Task New_Should_Pass()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Zizou13",
            "zinedine.zidane@marseille.fr",
            "ZiZou1998$");

        // Act
        await _authService.Register(command);
    }

    [Fact]
    public async Task Existing_Username_Should_Throw()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Ak2", "fake.address@domain.com", "QwErTy#$01");


        // Act + Assert
        await Assert.ThrowsAsync<AlreadyExistDomainException>(
            async () => await _authService.Register(command)
        );
    }

    [Fact]
    public async Task Existing_Email_Should_Throw()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Username", "alexcharbo@orange.fr", "QwErTy#$01");


        // Act + Assert
        await Assert.ThrowsAsync<AlreadyExistDomainException>(
            async () => await _authService.Register(command)
        );
    }

    [Fact]
    public async Task Password_Has_No_Special_Character_Should_Throw()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Zizou13",
            "zinedine.zidane@marseille.fr",
            "Password2023");

        // Act + Assert
        await Assert.ThrowsAsync<DataValidationException>(async () => await _authService.Register(command));
    }

    [Fact]
    public async Task Password_Has_No_Upper_Character_Should_Throw()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Zizou13",
            "zinedine.zidane@marseille.fr",
            "password2023#$");

        // Act + Assert
        await Assert.ThrowsAsync<DataValidationException>(async () => await _authService.Register(command));
    }

    [Fact]
    public async Task Password_Has_No_Lower_Character_Should_Throw()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Zizou13",
            "zinedine.zidane@marseille.fr",
            "PASSWORD2023#$");

        // Act + Assert
        await Assert.ThrowsAsync<DataValidationException>(async () => await _authService.Register(command));
    }

    [Fact]
    public async Task Password_Too_Small_Characters_Should_Throw()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Zizou13",
            "zinedine.zidane@marseille.fr",
            "Pa2#");

        // Act + Assert
        await Assert.ThrowsAsync<DataValidationException>(async () => await _authService.Register(command));
    }
}