using MongoDB.Bson;
using MongoDB.Driver;

namespace BlueGoat.MongoDBUtils
{
    public class HealthService
    {
        private readonly IMongoClientFactory clientFactory;
        internal static JsonCommand<BsonDocument> DbHealthCheckCommand = new("{ dbStats: 1}");

        public HealthService(IMongoClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public IDictionary<string,object> GetDbStats(string connection, string databaseName)
        {
            try
            {
                var client = clientFactory.GetClient(connection);
                var db = client.GetDatabase(databaseName);
                var stats = db.RunCommand(DbHealthCheckCommand);
                return stats.ToDictionary();
            }
            catch (MongoAuthenticationException)
            {
                throw new CommandException("Unable to authenticate with provided username/password");
            }
            catch (TimeoutException)
            {
                throw new CommandException("Timeout occurred while trying to connect to MongoDB instance");
            }
        }

        public long GetSize(string connection, string databaseName)
        {
            var stats = GetDbStats(connection, databaseName);
            var size = (decimal) Convert.ChangeType(stats["dataSize"], typeof(decimal));
            return Convert.ToInt64(size);
        }
    }
}
