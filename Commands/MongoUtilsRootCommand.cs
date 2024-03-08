using System.CommandLine;

namespace BlueGoat.MongoDBUtils.Commands
{
    public class MongoUtilsRootCommand : RootCommand
    {
        public MongoUtilsRootCommand(IMongoClientFactory clientFactory, IMigrationRunner migrationRunner) : base("MongoDB Utilities")
        {
            Name = "mongo-utils";

            AddGlobalOption(MongoUtilOptions.Connection);
            AddGlobalOption(new Option<bool>("--debug") { IsHidden = true });
            TreatUnmatchedTokensAsErrors = true;

            var healthService = new HealthService(clientFactory);
            var migrationCommand = new MigrationCommand(migrationRunner);
            var dropDatabaseCommand = new DropDatabaseCommand(clientFactory);

            Add(migrationCommand);
            Add(dropDatabaseCommand);
            Add(new ResetDatabaseCommand(dropDatabaseCommand, migrationCommand));
            Add(new HealthCheckCommand(healthService));
            Add(new ScenarioCommand(clientFactory, healthService));
        }

        public sealed override string Name
        {
            get => base.Name;
            set => base.Name = value;
        }
    }
}
