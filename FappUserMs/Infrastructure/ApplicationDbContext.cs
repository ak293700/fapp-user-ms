using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure;

public class ApplicationDbContext
{
    private const string DatabaseName = "users";
    private const string CollectionName = "users";

    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;

    public IMongoCollection<User> Users { get; }


    public ApplicationDbContext(string? connectionString)
    {
        if (connectionString is null)
            throw new Exception("Connection string is null");
        
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(DatabaseName);
        Users = _database.GetCollection<User>(CollectionName);
    }

    public ApplicationDbContext(IConfiguration configuration) 
        : this(configuration.GetConnectionString("UserMongoDb"))
    {
    }
}