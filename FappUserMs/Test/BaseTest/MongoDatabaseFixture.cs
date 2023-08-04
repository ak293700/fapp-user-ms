using FappCommon.MongoDbContext.Mock;
using Mongo2Go;
using Test.Mocks;

namespace Test.BaseTest;

/// <summary>
/// Singleton share between all tests
/// </summary>
public class MongoDatabaseFixture : IDisposable
{
    private readonly MongoDbRunner _runner;

    public MongoDatabaseFixture()
    {
        _runner = MongoDbRunner.Start();
    }

    public MockMongoDbContext GenerateDatabase()
    {
        return BaseMockMongoDbContext
            .GenerateDatabaseFromConnectionString<MockMongoDbContext>(_runner.ConnectionString);
    }

    public void ImportData(string databaseName, string collectionName, string inputFile)
    {
        _runner.Import(databaseName, collectionName, inputFile, false);
    }

    public void Dispose()
    {
        _runner.Dispose();
        GC.SuppressFinalize(this);
    }
}