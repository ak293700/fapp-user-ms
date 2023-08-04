using Application.Common.Interfaces;
using Infrastructure;
using Test.Mocks;

namespace Test.BaseTest;

/// <summary>
/// Every inheritor of this class will have a <see cref="IApplicationDbContext"/> seeded according to infrastructure seed
/// </summary>
[Collection(MongoDatabaseCollection.Name)]
public abstract class BaseSeedTest : IDisposable
{
    private readonly MongoDatabaseFixture _fixture;
    protected IApplicationDbContext Context => _context;
    private readonly ApplicationDbContextMock _context;


    protected BaseSeedTest(MongoDatabaseFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.GenerateDatabase();
        // SeedAsync().Wait();
    }

    protected void SeedUsers()
    {
        _fixture.ImportData(_context.DatabaseName, ApplicationDbContext.UserCollectionName, "");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}