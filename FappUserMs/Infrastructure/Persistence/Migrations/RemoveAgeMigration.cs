using Domain.Entities;
using MongoDB.Driver;
using MongoDBMigrations;
using Version = MongoDBMigrations.Version;

namespace Infrastructure.Persistence.Migrations;

public class RemoveAgeMigration : IMigration
{
    public Version Version => new Version(0, 2, 0);
    public string Name => "RemoveAgeMigration";

    public void Up(IMongoDatabase database)
    {
        IMongoCollection<User> users = database.GetCollection<User>(ApplicationDbContext.UserCollectionName);
        users.UpdateMany(Builders<User>.Filter.Empty, Builders<User>.Update.Unset("Age"));
    }

    public void Down(IMongoDatabase database)
    {
        IMongoCollection<User> users = database.GetCollection<User>(ApplicationDbContext.UserCollectionName);
        users.UpdateMany(Builders<User>.Filter.Empty, Builders<User>.Update.Set("Age", 18));
    }
}