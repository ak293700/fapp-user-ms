using Application.Common.Interfaces;
using Domain.Entities;
using FappCommon.MongoDbContext.Mock;
using Infrastructure;
using MongoDB.Driver;

namespace Test.Mocks;

public class MockMongoDbContext : BaseMockMongoDbContext, IApplicationDbContext, IDisposable
{
    public IMongoCollection<User> Users { get; private set; } = null!;

    public MockMongoDbContext()
    {
    }


    public override void InitCollections(IMongoDatabase database)
    {
        Users = database.GetCollection<User>(ApplicationDbContext.UserCollectionName);
    }

    public override void RunMigrations(string connectionString)
    {
        ApplicationDbContext.RunMigrations(connectionString, DatabaseName);
    }


    public void Dispose()
    {
        // Just there to save memory during the test process
        // At the end the instance is dropped so
        Client.DropDatabase(DatabaseName);
        GC.SuppressFinalize(this);
    }
}