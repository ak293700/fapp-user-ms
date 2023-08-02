using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDBMigrations;

namespace Infrastructure;

public class ApplicationDbContext : IApplicationDbContext, IDisposable
{
    public IMongoCollection<User> Users { get; }

    private readonly ILoggerFactory _loggerFactory;
    public const string DatabaseName = "users";
    public const string UserCollectionName = "users";

    public ApplicationDbContext(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("UserMongoDb")
                                  ?? throw new Exception("Connection string is null");

        _loggerFactory = CreateLoggerFactory(configuration);

        IMongoDatabase? database = CreateClient(connectionString).GetDatabase(DatabaseName);
        Users = database.GetCollection<User>(UserCollectionName);
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

    public static void RunMigrations(IConfiguration configuration)
    {
        // Get the connection string from the appsettings.json file
        string connectionString = configuration.GetConnectionString("UserMongoDb")
                                  ?? throw new Exception("Connection string is null");

        new MigrationEngine()
            .UseDatabase(connectionString, DatabaseName)
            .UseAssemblyOfType<ApplicationDbContext>()
            .UseSchemeValidation(false)
            .Run();
    }

    public void Dispose()
    {
        _loggerFactory.Dispose();
        // GC.SuppressFinalize(this); // TODO: Understand this
    }
}