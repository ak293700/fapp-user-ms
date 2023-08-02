using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure;
using MongoDB.Driver;

namespace Test.Mocks;

public class ApplicationDbContextMock : IApplicationDbContext, IDisposable
{
    public IMongoCollection<User> Users { get; }

    private readonly string _databaseName;
    private readonly IMongoClient _client;

    private ApplicationDbContextMock(string connectionString)
    {
        #region Pre-Initialization

        _client = new MongoClient(connectionString);
        _databaseName = Guid.NewGuid().ToString();
        IMongoDatabase database = _client.GetDatabase(_databaseName);
        ApplicationDbContext.RunMigrations(connectionString, _databaseName);

        #endregion

        Users = database.GetCollection<User>(ApplicationDbContext.UserCollectionName);
    }

    public static ApplicationDbContextMock GenerateDatabaseFromConnectionString(string connectionString)
    {
        return new ApplicationDbContextMock(connectionString);
    }


    public void Dispose()
    {
        _client.DropDatabase(_databaseName);
        GC.SuppressFinalize(this);
    }
}