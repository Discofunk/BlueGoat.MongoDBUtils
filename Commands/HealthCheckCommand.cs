using System.CommandLine;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace BlueGoat.MongoDBUtils.Commands
{
    public class HealthCheckCommand : Command
    {
        private readonly HealthService healthService;

        public HealthCheckCommand(HealthService healthService) : base("health", "Runs a Health Check against a MongoDB instance")
        {
            this.healthService = healthService;
            AddOption(MongoUtilOptions.DatabaseName);
            this.SetHandler(HealthCheck, MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName);
        }

        private void HealthCheck(string connection, string databaseName)
        {
            var stats = healthService.GetDbStats(connection, databaseName);
            var pretty = stats.ToJson(new JsonWriterSettings() { Indent = true });
            ConsoleEx.WriteLineOk($"Connected!");
            ConsoleEx.WriteLine(pretty);
        }
    }
}
