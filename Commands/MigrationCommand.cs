using System.CommandLine;
using System.Reflection;

namespace BlueGoat.MongoDBUtils.Commands;

public class MigrationCommand : Command
{
    public MigrationCommand() : base("migrate", "Run MongoDB migration")
    {
        AddOption(MongoUtilOptions.DatabaseName);
        AddOption(MongoUtilOptions.Version);
        AddOption(MongoUtilOptions.MigrationAssembly);
        this.SetHandler(RunMigration, MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName, MongoUtilOptions.MigrationAssembly, MongoUtilOptions.Version);
    }

    internal static void RunMigration(string connection, string databaseName, FileInfo migrationAssemblyPath, string? version = null)
    {
        var runner = new MigrationRunner();
        var assembly = Assembly.LoadFrom(migrationAssemblyPath.FullName);
        runner.RunMigrations(assembly, connection, databaseName, version, progressAction: result => Console.WriteLine($"Running {result.CurrentNumber} of {result.TotalCount}: {result.MigrationName}"));
        Console.WriteLine($"Migrations Run");
    }

}