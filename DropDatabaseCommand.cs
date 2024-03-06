using System.CommandLine;
using MongoDB.Driver;

namespace BlueGoat.MongoDBUtils;

public class DropDatabaseCommand : Command
{
    public DropDatabaseCommand() : base("drop", "Drop a MongoDB database")
    {
        AddOption(MongoUtilOptions.DatabaseName);
        AddOption(MongoUtilOptions.ForceOption);
        this.SetHandler((connection, databaseName, force) => DropDatabase(connection, databaseName, force), 
            MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.ForceOption);
    }

    internal static bool DropDatabase(string connection, string databaseName, bool force)
    {
        if (!force)
        {
            ConsoleEx.WriteWarn($"Dropping database will permanently delete all data. Are you sure you want to continue? [Y]es / [N]o: ");
            var response = Console.ReadLine()?.ToUpper();
            if (response != "Y") return false;
        }

        var client = new MongoClient(connection);
        client.DropDatabase(databaseName);
        Console.WriteLine($"Database {databaseName} dropped");
        return true;
    }
}