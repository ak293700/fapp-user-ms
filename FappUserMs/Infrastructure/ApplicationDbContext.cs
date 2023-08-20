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

/*public class ApplicationDbContext : IApplicationDbContext, IDisposable
{
    public IMongoCollection<User> Users { get; }

    private readonly ILoggerFactory _loggerFactory;
    public const string ConnectionStringName = "UserMongoDb";
    public const string DatabaseName = "users";
    public const string UserCollectionName = "users";

    public ApplicationDbContext(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString(ConnectionStringName)
                                  ?? throw ConfigurationException.ValueNotFoundException.Instance;


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
        string connectionString = configuration.GetConnectionString(ConnectionStringName)
                                  ?? throw ConfigurationException.ValueNotFoundException.Instance;

        RunMigrations(connectionString, DatabaseName);
    }

    public static void RunMigrations(string connectionString, string databaseName)
    {
        new MigrationEngine()
            .UseDatabase(connectionString, databaseName)
            .UseAssemblyOfType<ApplicationDbContext>()
            .UseSchemeValidation(false)
            .Run();
    }


    public void Dispose()
    {
        _loggerFactory.Dispose();
        GC.SuppressFinalize(this);
    }
}*/