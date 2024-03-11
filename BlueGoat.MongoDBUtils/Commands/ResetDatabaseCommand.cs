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
        this.SetHandler((connection, databaseName, force, migrationAssembly) => 
            ResetDatabase(connection, databaseName, force, migrationAssembly), 
            MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.ForceOption, MongoUtilOptions.MigrationAssembly
        );
    }

    private Result ResetDatabase(string connection, string databaseName, bool force, FileInfo migrationAssemblyPath)
    {
        var dropResult = dropDatabaseCommand.DropDatabase(connection, databaseName, force);
        return dropResult != Result.Success ? 
            dropResult : 
            migrationCommand.RunMigration(connection, databaseName, migrationAssemblyPath);
    }
}