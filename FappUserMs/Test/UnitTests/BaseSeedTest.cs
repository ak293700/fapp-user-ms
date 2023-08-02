using Application.Common.Interfaces;

namespace Test.UnitTests;

/// <summary>
/// Every inheritor of this class will have a <see cref="IApplicationDbContext"/> seeded according to infrastructure seed
/// </summary>
public abstract class BaseSeedTest
{
    protected readonly IApplicationDbContext Context;
    // protected readonly ApplicationDbContextInitializer Initializer;

    public BaseSeedTest()
    {
        Context = ContextGenerator.GenerateInMemory();
        // Initializer = new ApplicationDbContextInitializer(Context);
        // Initializer.SeedAsync().Wait();
    }
}