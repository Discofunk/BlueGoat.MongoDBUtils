using System.CommandLine;

namespace BlueGoat.MongoDBUtils.Commands;

public class ScenarioCommand : Command
{
    public ScenarioCommand(IMongoClientFactory clientFactory, HealthService healthService) : base("scenario", "Scenario Commands")
    {
        AddCommand(new SaveScenarioCommand(clientFactory, healthService));
        AddCommand(new LoadScenarioCommand(clientFactory));
    }
}