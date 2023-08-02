using Domain.Entities;
using MongoDB.Driver;
using MongoDBMigrations;
using Version = MongoDBMigrations.Version;

namespace Infrastructure.Persistence.Migrations;

public class FakeMigration : IMigration
{
    public Version Version => new Version(0, 1, 0);
    public string Name => "FakeMigration";

    public void Up(IMongoDatabase database)
    {
        IMongoCollection<User> users = database.GetCollection<User>(ApplicationDbContext.UserCollectionName);
        users.UpdateMany(Builders<User>.Filter.Empty, Builders<User>.Update.Set("Age", 18));
    }

    public void Down(IMongoDatabase database)
    {
        IMongoCollection<User> users = database.GetCollection<User>(ApplicationDbContext.UserCollectionName);
        users.UpdateMany(Builders<User>.Filter.Empty, Builders<User>.Update.Unset("Age"));
    }
}