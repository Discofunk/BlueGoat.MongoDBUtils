using System.CommandLine;

namespace BlueGoat.MongoDBUtils.Commands;

public class DropDatabaseCommand : Command
{
    private readonly IMongoClientFactory clientFactory;
    private readonly IConsole console;

    public DropDatabaseCommand(IMongoClientFactory clientFactory, IConsole console) : base("drop", "Drop a MongoDB database")
    {
        this.clientFactory = clientFactory;
        this.console = console;
        AddOption(MongoUtilOptions.DatabaseName);
        AddOption(MongoUtilOptions.ForceOption);
        this.SetHandler((connection, databaseName, force) => DropDatabase(connection, databaseName, force),
            MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.ForceOption);
    }

    internal Result DropDatabase(string connection, string databaseName, bool force)
    {
        if (!force)
        {
            console.WriteWarn($"Dropping database will permanently delete all data. Are you sure you want to continue? [Y]es / [N]o: ");
            var response = console.ReadLine()?.ToUpper();
            if (response != "Y") return Result.Cancelled;
        }

        var client = clientFactory.GetClient(connection);
        client.DropDatabase(databaseName);
        console.WriteLine($"Database {databaseName} dropped");
        return Result.Success;
    }
}