using FappCommon.MongoDbContext;
using Test.Mocks;

namespace Test.Base;

/// <summary>
/// Singleton share between all tests
/// </summary>
public class MongoDatabaseFixture : BaseMongoDatabaseFixture<MockMongoDbContext>
{
    public override MockMongoDbContext GenerateDatabase()
    {
        return BaseMockMongoDbContext
            .GenerateDatabaseFromConnectionString<MockMongoDbContext>(Runner.ConnectionString);
    }
}