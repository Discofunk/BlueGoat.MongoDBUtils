using System.CommandLine;

namespace BlueGoat.MongoDBUtils;

public class ResetDatabaseCommand : Command
{
    public ResetDatabaseCommand() : base("reset", "Drop database and run all migrations")
    {
        AddOption(MongoUtilOptions.DatabaseName);
        AddOption(MongoUtilOptions.ForceOption);
        AddOption(MongoUtilOptions.MigrationAssembly);
        this.SetHandler(ResetDatabase, MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.ForceOption, MongoUtilOptions.MigrationAssembly);
    }

    private void ResetDatabase(string connection, string databaseName, bool force, FileInfo migrationAssemblyPath)
    {
        if (!DropDatabaseCommand.DropDatabase(connection, databaseName, force)) return;
        MigrationCommand.RunMigration(connection, databaseName, migrationAssemblyPath);
    }
}