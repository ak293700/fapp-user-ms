using Application.Common.Interfaces;
using Infrastructure;
using Mongo2Go;
using MongoDB.Driver;
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
    public static IApplicationDbContext GenerateInMemory()
    {
        if (_runner is null)
            _runner = MongoDbRunner.StartForDebugging(port: 27018);

        // Log the connection string to the test console
        Console.WriteLine();

        // Generate a new database for each test
        string databaseName = Guid.NewGuid().ToString();
        IMongoDatabase database = new MongoClient(_runner.ConnectionString).GetDatabase(databaseName);
        ApplicationDbContext.RunMigrations(_runner.ConnectionString, databaseName);

        return new ApplicationDbContextMock(database);
    }
}