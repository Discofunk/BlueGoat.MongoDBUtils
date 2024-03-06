using System.CommandLine;

namespace MongoDBUtils;

public class ScenarioCommand : Command
{
    public ScenarioCommand() : base("scenario", "Scenario Commands")
    {
        AddCommand(new SaveScenarioCommand());
        AddCommand(new LoadScenarioCommand());
    }
}