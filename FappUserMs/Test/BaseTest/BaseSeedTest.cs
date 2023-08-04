using Application.Common.Interfaces;
using Test.Mocks;

namespace Test.BaseTest;

/// <summary>
/// Every inheritor of this class will have a <see cref="IApplicationDbContext"/> seeded according to infrastructure seed
/// </summary>
[Collection(MongoDatabaseCollection.Name)]
public abstract class BaseSeedTest : IDisposable
{
    protected IApplicationDbContext Context => _context;

    private readonly ApplicationDbContextMock _context;
    // protected readonly ApplicationDbContextInitializer Initializer;

    public BaseSeedTest(MongoDatabaseFixture fixture)
    {
        _context = fixture.GenerateDatabase();
        // _context = ContextGenerator.GenerateInMemory();
        // Initializer = new ApplicationDbContextInitializer(Context);
        // Initializer.SeedAsync().Wait();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}