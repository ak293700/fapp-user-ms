using Application.Common.Interfaces;
using Domain.Entities.UserEntities;
using FappCommon.Mongo4Test.Implementations._4Tests;
using Infrastructure;
using MongoDB.Driver;

namespace Test.Base;

public class MockMongoDbContext : BaseMockMongoDbContext, IApplicationDbContext
{
    public IMongoCollection<User> Users { get; private set; } = null!;


    protected override void InitCollections(IMongoDatabase database)
    {
        Users = database.GetCollection<User>(ApplicationDbContext.UserCollectionName);
    }

    protected override void RunMigrations(string connectionString)
    {
        // Run
        // ApplicationDbContext.RunMigrations(connectionString, DatabaseName);
    }
}