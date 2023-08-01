using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure;

public class ApplicationDbContext : IApplicationDbContext
{
    public IMongoCollection<User> Users { get; }


    public ApplicationDbContext(string? connectionString)
    {
        if (connectionString is null)
            throw new Exception("Connection string is null");
        
        const string databaseName = "users"; 
        MongoClient client = new MongoClient(connectionString);
        
        const string collectionName = "users";
        IMongoDatabase? database = client.GetDatabase(databaseName);
        
        Users = database.GetCollection<User>(collectionName);
    }

    public ApplicationDbContext(IConfiguration configuration) 
        : this(configuration.GetConnectionString("UserMongoDb"))
    {
    }
}