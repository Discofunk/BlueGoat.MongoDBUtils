using System.CommandLine;

namespace BlueGoat.MongoDBUtils.Commands
{
    public class MongoUtilsRootCommand : RootCommand
    {
        public MongoUtilsRootCommand() : base("MongoDB Utilities")
        {
            Name = "mongo-utils";

            AddGlobalOption(MongoUtilOptions.Connection);
            AddGlobalOption(new Option<bool>("--debug") { IsHidden = true });
            TreatUnmatchedTokensAsErrors = true;

            Add(new MigrationCommand());
            Add(new DropDatabaseCommand());
            Add(new ResetDatabaseCommand());
            Add(new HealthCheckCommand());
            Add(new ScenarioCommand());
        }

        public sealed override string Name
        {
            get => base.Name;
            set => base.Name = value;
        }
    }
}
