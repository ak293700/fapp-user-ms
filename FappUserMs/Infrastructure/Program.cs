// See https://aka.ms/new-console-template for more information

using Infrastructure;
using MongoDBMigrations;

Console.WriteLine("Hello, World!");

// Get the connection string from the appsettings.json file
const string connectionString = "mongodb://root:password@localhost:27017/";

new MigrationEngine()
    .UseDatabase(connectionString, ApplicationDbContext.DatabaseName)
    .UseAssemblyOfType<ApplicationDbContext>()
    .UseSchemeValidation(false)
    .Run();