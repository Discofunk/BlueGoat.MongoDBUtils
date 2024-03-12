using System.CommandLine;
using System.Diagnostics;
using BlueGoat.MongoDBUtils.Commands;

namespace BlueGoat.MongoDBUtils;

public class Program
{
    public static async Task<int> Main(params string[] args)
    {
        if (args.Any(x => x.Contains("--debug")))
        {
            Debugger.Launch();
        }

        var rootCommand = new MongoUtilsRootCommand(new MongoClientProvider(), new MigrationRunner(), new ConsoleEx());

        try
        {
            var result = await rootCommand.InvokeAsync(args);
            return result;
        }
        finally
        {
            Console.ResetColor();
        }
    }
}