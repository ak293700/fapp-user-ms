using FappCommon.MongoDbContext;
using Mongo2Go;
using Test.Mocks;

namespace Test.BaseTest;

/// <summary>
/// Singleton share between all tests
/// </summary>
public class MongoDatabaseFixture : BaseMongoDatabaseFixture<MockMongoDbContext>
{
    private readonly MongoDbRunner _runner;

    public MongoDatabaseFixture()
    {
        _runner = MongoDbRunner.Start();
    }


    public override MockMongoDbContext GenerateDatabase()
    {
        return BaseMockMongoDbContext
            .GenerateDatabaseFromConnectionString<MockMongoDbContext>(_runner.ConnectionString);
    }
}