using System.CommandLine;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace BlueGoat.MongoDBUtils.Commands
{
    public class HealthCheckCommand : Command
    {
        public HealthCheckCommand() : base("health", "Runs a Health Check against a MongoDB instance")
        {
            AddOption(MongoUtilOptions.DatabaseName);
            this.SetHandler(HealthCheck, MongoUtilOptions.Connection, MongoUtilOptions.DatabaseName);
        }

        private void HealthCheck(string connection, string databaseName)
        {

            var stats = HealthService.GetDbStats(connection, databaseName);
            var pretty = stats.ToJson(new JsonWriterSettings() { Indent = true });
            ConsoleEx.WriteLineOk($"Connected!");
            ConsoleEx.WriteLine(pretty);
        }
    }
}
