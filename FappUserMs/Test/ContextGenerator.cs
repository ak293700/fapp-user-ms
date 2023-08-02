using Mongo2Go;
using Test.Mocks;

namespace Test;

public static class ContextGenerator
{
    // Mongo instance shared by every tests
    private static MongoDbRunner? _runner = null;

    /// <summary>
    /// Generate an in memory database context.
    /// An in memory does not check for constraint violations as foreign keys are not enforced.
    /// </summary>
    public static ApplicationDbContextMock GenerateInMemory()
    {
        if (_runner is null)
            _runner = MongoDbRunner.StartForDebugging(port: 27018);

        // Generate a new database for each test
        return ApplicationDbContextMock.GenerateDatabaseFromConnectionString(_runner.ConnectionString);
    }
}