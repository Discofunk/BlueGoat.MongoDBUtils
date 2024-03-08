using System.CommandLine;
using System.Reflection;

namespace BlueGoat.MongoDBUtils.Commands;

public class MigrationCommand : Command
{
    private readonly IMigrationRunner migrationRunner;

    public MigrationCommand(IMigrationRunner migrationRunner) : base("migrate", "Run MongoDB migration")
    {
        this.migrationRunner = migrationRunner;
        AddOption(MongoUtilOptions.DatabaseName);
        AddOption(MongoUtilOptions.Version);
        AddOption(MongoUtilOptions.MigrationAssembly);
        this.SetHandler(RunMigration, MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.MigrationAssembly, MongoUtilOptions.Version);
    }

    internal void RunMigration(string connection, string databaseName, FileInfo migrationAssemblyPath, string? version = null)
    {
        var assembly = Assembly.LoadFrom(migrationAssemblyPath.FullName);
        migrationRunner.RunMigrations(assembly, connection, databaseName, version, progressAction: result => Console.WriteLine($"Running {result.CurrentNumber} of {result.TotalCount}: {result.MigrationName}"));
        Console.WriteLine($"Migrations Run");
    }

}