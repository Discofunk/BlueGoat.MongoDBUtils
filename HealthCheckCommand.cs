using System.CommandLine;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace MongoDBUtils
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
            try
            {
                var client = new MongoClient(connection);
                var db = client.GetDatabase(databaseName);
                //var result = db.RunCommand<BsonDocument>("{ buildInfo: 1 }");
                //var version = result.GetValue("version");

                var stats = db.RunCommand<BsonDocument>("{ dbStats: 1}");
                var pretty = stats.ToJson(new JsonWriterSettings() {Indent = true});
                ConsoleEx.WriteLineOk($"Connected!");
                ConsoleEx.WriteLine(pretty);
            }
            catch (MongoAuthenticationException)
            {
                ConsoleEx.WriteLineWarn("Unable to authenticate with provided username/password");
            }
            catch (TimeoutException)
            {
                ConsoleEx.WriteLineError("Timeout occurred while trying to connect to MongoDB instance");
            }
        }
    }
}
