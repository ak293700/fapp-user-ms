using Application.Common.Dtos.AuthDtos;
using Application.Common.Exceptions;
using Application.Services;
using Test.Base;
using Test.Mocks;

namespace Test.UnitTests.Application.Services.AuthServiceTests;

public class RegisterTest : BaseTest
{
    private readonly AuthService _authService;

    private static readonly Dictionary<string, string?> MockConf = new Dictionary<string, string?>
    {
        { "Auth:Token", "aSeCrEtToKeN" }
    };

    public RegisterTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        SeedUsers();

        _authService = new AuthService(Context, ConfigurationMock.GetCustom(MockConf));
    }

    [Fact]
    public async Task New_Should_Pass()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Zizou59", "zizou@marseille.msl", "ZiZou1998$");

        // Act
        await _authService.Register(command);
    }

    [Fact]
    public async Task Password_Has_No_Special_Character_Should_Throw()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Zizou59", "zizou@marseille.msl", "ZiZou1998");

        // Act + Assert
        await Assert.ThrowsAsync<DataValidationException>(async () => await _authService.Register(command));
    }

    [Fact]
    public async Task Same_Username_Should_Pass()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Ak2", "address@domain.com", "QwErTy#$01");

        // Act + Assert
        await Assert.ThrowsAsync<MongoDB.Driver.MongoWriteException>(
            async () => await _authService.Register(command)
        );
    }
}