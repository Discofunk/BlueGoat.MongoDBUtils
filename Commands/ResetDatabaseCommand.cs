using System.CommandLine;

namespace BlueGoat.MongoDBUtils.Commands;

public class ResetDatabaseCommand : Command
{
    private readonly DropDatabaseCommand dropDatabaseCommand;
    private readonly MigrationCommand migrationCommand;

    public ResetDatabaseCommand(DropDatabaseCommand dropDatabaseCommand, MigrationCommand migrationCommand) : base("reset", "Drop database and run all migrations")
    {
        this.dropDatabaseCommand = dropDatabaseCommand;
        this.migrationCommand = migrationCommand;
        AddOption(MongoUtilOptions.DatabaseName);
        AddOption(MongoUtilOptions.ForceOption);
        AddOption(MongoUtilOptions.MigrationAssembly);
        this.SetHandler(ResetDatabase, MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.ForceOption, MongoUtilOptions.MigrationAssembly);
    }

    private void ResetDatabase(string connection, string databaseName, bool force, FileInfo migrationAssemblyPath)
    {
        if (!dropDatabaseCommand.DropDatabase(connection, databaseName, force)) return;
        migrationCommand.RunMigration(connection, databaseName, migrationAssemblyPath);
    }
}