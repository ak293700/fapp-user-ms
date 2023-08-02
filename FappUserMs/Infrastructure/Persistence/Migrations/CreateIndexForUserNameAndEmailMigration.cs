using Domain.Entities;
using MongoDB.Driver;
using MongoDBMigrations;

namespace Infrastructure.Persistence.Migrations;

public class CreateIndexForUserNameAndEmailMigration : IMigration
{
    public Version Version => new Version(0, 3, 0);
    public string Name => "CreateIndexForUserNameAndEmail";

    public void Up(IMongoDatabase database)
    {
        IMongoCollection<User> users = database.GetCollection<User>(ApplicationDbContext.UserCollectionName);
        users.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.UserName),
                new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true })
        });
    }

    public void Down(IMongoDatabase database)
    {
        IMongoCollection<User> users = database.GetCollection<User>(ApplicationDbContext.UserCollectionName);
        users.Indexes.DropOne("UserName");
        users.Indexes.DropOne("Email");
    }
}