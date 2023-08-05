using Application.Common.Dtos.AuthDtos;
using Application.Common.Exceptions;
using Application.Services;
using Domain.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
    public async Task Same_Username_Should_Throw()
    {
        // Arrange
        RegisterDto command = new RegisterDto("Ak2", "address@domain.com", "QwErTy#$01");

        var count = await Context.Users.AsQueryable()
            .CountAsync();

        var client = Context.Client;
        var database = client.GetDatabase(Context.DatabaseName);
        var collection = database.GetCollection<User>("users");

        var count2 = await collection.AsQueryable()
            .CountAsync();

        // Act + Assert
        await Assert.ThrowsAsync<MongoWriteException>(
            async () => await _authService.Register(command)
        );
    }

    [Fact]
    public async Task Twice_Same_User_Should_Throw()
    {
        // Arrange
        RegisterDto command1 = new RegisterDto("Ak2", "address@domain.com", "QwErTy#$01");
        RegisterDto command2 = new RegisterDto("Ak2", "address2@domain.com", "QwErTy#$01");

        await _authService.Register(command1);

        // Act + Assert
        await Assert.ThrowsAsync<MongoWriteException>(
            async () => await _authService.Register(command2)
        );
    }
}