using System.CommandLine;
using System.Diagnostics;

namespace BlueGoat.MongoDBUtils;

public class Program
{
    static async Task<int> Main(params string[] args)
    {
        if (args.Any(x => x.Contains("--debug")))
        {
            Debugger.Launch();
        }

        var rootCommand = new RootCommand("MongoDB Utilities")
        {
            new MigrationCommand(),
            new DropDatabaseCommand(),
            new ResetDatabaseCommand(),
            new HealthCheckCommand(),
            new ScenarioCommand()
        };
        rootCommand.Name = "mongo-utils";
        rootCommand.AddGlobalOption(MongoUtilOptions.Connection);
        rootCommand.AddGlobalOption(new Option<bool>("--debug") { IsHidden = true });

        rootCommand.TreatUnmatchedTokensAsErrors = true;

        var result = await rootCommand.InvokeAsync(args);
        Console.ResetColor();
        return result;
    }
}