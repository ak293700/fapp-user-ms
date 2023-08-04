using Mongo2Go;
using Test.Mocks;

namespace Test.BaseTest;

public class MongoDatabaseFixture : IDisposable
{
    private readonly MongoDbRunner _runner;

    public MongoDatabaseFixture()
    {
        _runner = MongoDbRunner.Start();
    }

    public ApplicationDbContextMock GenerateDatabase()
    {
        return ApplicationDbContextMock.GenerateDatabaseFromConnectionString(_runner.ConnectionString);
    }


    public void Dispose()
    {
        _runner.Dispose();
        GC.SuppressFinalize(this);
    }
}