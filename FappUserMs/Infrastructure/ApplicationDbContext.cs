using Application.Common.Interfaces;
using Domain.Entities.UserEntities;
using FappCommon.Mongo4Test.Implementations;
using MongoDB.Driver;

namespace Infrastructure;

public class ApplicationDbContext : BaseMongoDbContext, IApplicationDbContext
{
    public const string UserCollectionName = "users";
    public IMongoCollection<User> Users { get; private set; } = null!;

    protected override void InitializeCollections(IMongoDatabase database)
    {
        Users = database.GetCollection<User>(UserCollectionName);
    }
}