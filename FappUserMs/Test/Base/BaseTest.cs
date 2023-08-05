using Application.Common.Interfaces;
using FappCommon.Mongo4Test;
using Infrastructure;

namespace Test.Base;

/// <summary>
/// Every inheritor of this class will have a <see cref="IApplicationDbContext"/> seeded according to infrastructure seed
/// </summary>
[Collection(MongoDatabaseCollection.Name)]
public abstract class BaseTest : BaseMongoTest<MockMongoDbContext>
{
    protected BaseTest(MongoDatabaseFixture fixture) : base(fixture)
    {
        SeedUsers();
    }


    protected void SeedUsers()
    {
        Fixture.ImportData(Context.DatabaseName, ApplicationDbContext.UserCollectionName,
            "/Users/alexandreakdeniz/Documents/Projets/fapp/fapp-user-ms/FappUserMs/db-initializer/users.json");
    }
}