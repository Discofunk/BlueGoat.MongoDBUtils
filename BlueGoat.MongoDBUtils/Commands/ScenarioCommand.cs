using System.CommandLine;

namespace BlueGoat.MongoDBUtils.Commands;

public class ScenarioCommand : Command
{
    public ScenarioCommand(IMongoClientProvider clientProvider, HealthService healthService, IConsole console) : base("scenario", "Scenario Commands")
    {
        AddCommand(new SaveScenarioCommand(clientProvider, healthService, console));
        AddCommand(new LoadScenarioCommand(clientProvider, console));
    }
}