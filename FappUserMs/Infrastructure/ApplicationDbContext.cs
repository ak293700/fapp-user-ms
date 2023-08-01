using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace Infrastructure;

public class ApplicationDbContext : IApplicationDbContext, IDisposable
{
    public IMongoCollection<User> Users { get; }

    private readonly ILoggerFactory _loggerFactory;


    public ApplicationDbContext(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("UserMongoDb")
                                  ?? throw new Exception("Connection string is null");

        _loggerFactory = CreateLoggerFactory(configuration);

        const string databaseName = "users";
        IMongoDatabase? database = CreateClient(connectionString).GetDatabase(databaseName);

        const string collectionName = "users";
        Users = database.GetCollection<User>(collectionName);
    }

    private ILoggerFactory CreateLoggerFactory(IConfiguration configuration)
    {
        return LoggerFactory.Create(lb =>
        {
            lb.AddConfiguration(configuration.GetSection("Logging"));
            lb.AddSimpleConsole();
        });
    }

    private MongoClient CreateClient(string connectionString)
    {
        MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);
        settings.LoggingSettings = new LoggingSettings(_loggerFactory);
        return new MongoClient(settings);
    }

    public void Dispose()
    {
        _loggerFactory.Dispose();
        // GC.SuppressFinalize(this); // TODO: Understand this
    }
}