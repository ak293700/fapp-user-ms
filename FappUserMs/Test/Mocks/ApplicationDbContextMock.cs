using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure;
using MongoDB.Driver;

namespace Test.Mocks;

public class ApplicationDbContextMock : IApplicationDbContext, IDisposable
{
    public IMongoCollection<User> Users { get; }

    public readonly string DatabaseName;
    private readonly IMongoClient _client;

    private ApplicationDbContextMock(string connectionString)
    {
        #region Pre-Initialization

        _client = new MongoClient(connectionString);
        DatabaseName = Guid.NewGuid().ToString();
        IMongoDatabase database = _client.GetDatabase(DatabaseName);
        ApplicationDbContext.RunMigrations(connectionString, DatabaseName);

        #endregion

        Users = database.GetCollection<User>(ApplicationDbContext.UserCollectionName);
    }

    public static ApplicationDbContextMock GenerateDatabaseFromConnectionString(string connectionString)
    {
        return new ApplicationDbContextMock(connectionString);
    }


    public void Dispose()
    {
        // Just there to same memory during the test process
        // At the end the instance is dropped so
        _client.DropDatabase(DatabaseName);
        GC.SuppressFinalize(this);
    }
}