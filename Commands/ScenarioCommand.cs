using System.CommandLine;

namespace BlueGoat.MongoDBUtils.Commands;

public class ScenarioCommand : Command
{
    public ScenarioCommand(IMongoClientFactory clientFactory, HealthService healthService, IConsole console) : base("scenario", "Scenario Commands")
    {
        AddCommand(new SaveScenarioCommand(clientFactory, healthService, console));
        AddCommand(new LoadScenarioCommand(clientFactory, console));
    }
}