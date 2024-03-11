using System.CommandLine;
using System.Reflection;

namespace BlueGoat.MongoDBUtils.Commands;

public class MigrationCommand : Command
{
    private readonly IMigrationRunner migrationRunner;
    private readonly IConsole console;

    public MigrationCommand(IMigrationRunner migrationRunner, IConsole console) : base("migrate", "Run MongoDB migration")
    {
        this.migrationRunner = migrationRunner;
        this.console = console;
        AddOption(MongoUtilOptions.DatabaseName);
        AddOption(MongoUtilOptions.Version);
        AddOption(MongoUtilOptions.MigrationAssembly);
        this.SetHandler((connection, databaseName, migrationAssemblyPath, version) => 
            RunMigration(connection, databaseName, migrationAssemblyPath, version), 
            MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.MigrationAssembly, MongoUtilOptions.Version
        );
    }

    internal Result RunMigration(string connection, string databaseName, FileInfo migrationAssemblyPath, string? version = null)
    {
        var assembly = Assembly.LoadFrom(migrationAssemblyPath.FullName);
        migrationRunner.RunMigrations(assembly, connection, databaseName, version, progressAction: result => console.WriteLine($"Running {result.CurrentNumber} of {result.TotalCount}: {result.MigrationName}"));
        console.WriteLine($"Migrations Run");
        return Result.Success;
    }

}