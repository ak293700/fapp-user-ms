using Application.Common.Interfaces;
using FappCommon.MongoDbContext;
using Infrastructure;
using Test.Mocks;

namespace Test.BaseTest;

/// <summary>
/// Every inheritor of this class will have a <see cref="IApplicationDbContext"/> seeded according to infrastructure seed
/// </summary>
[Collection(MongoDatabaseCollection.Name)]
public abstract class BaseTest : BaseMongoTest<MockMongoDbContext>, IDisposable
{
    protected IApplicationDbContext Context => DbContext;

    protected BaseTest(MongoDatabaseFixture fixture) : base(fixture)
    {
    }


    protected void SeedUsers()
    {
        Fixture.ImportData(DbContext.DatabaseName, ApplicationDbContext.UserCollectionName,
            "/Users/alexandreakdeniz/Documents/Projets/fapp/fapp-user-ms/FappUserMs/db-initializer/users.json");
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}