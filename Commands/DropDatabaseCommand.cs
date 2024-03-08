using System.CommandLine;

namespace BlueGoat.MongoDBUtils.Commands;

public class DropDatabaseCommand : Command
{
    private readonly IMongoClientFactory clientFactory;

    public DropDatabaseCommand(IMongoClientFactory clientFactory) : base("drop", "Drop a MongoDB database")
    {
        this.clientFactory = clientFactory;
        AddOption(MongoUtilOptions.DatabaseName);
        AddOption(MongoUtilOptions.ForceOption);
        this.SetHandler((connection, databaseName, force) => DropDatabase(connection, databaseName, force),
            MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.ForceOption);
    }

    internal bool DropDatabase(string connection, string databaseName, bool force)
    {
        if (!force)
        {
            ConsoleEx.WriteWarn($"Dropping database will permanently delete all data. Are you sure you want to continue? [Y]es / [N]o: ");
            var response = Console.ReadLine()?.ToUpper();
            if (response != "Y") return false;
        }

        var client = clientFactory.GetClient(connection);
        client.DropDatabase(databaseName);
        Console.WriteLine($"Database {databaseName} dropped");
        return true;
    }
}