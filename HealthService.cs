﻿using MongoDB.Bson;
using MongoDB.Driver;

namespace BlueGoat.MongoDBUtils
{
    public class HealthService
    {
        public static IDictionary<string,object> GetDbStats(string connection, string databaseName)
        {
            try
            {
                var client = new MongoClient(connection);
                var db = client.GetDatabase(databaseName);
                var stats = db.RunCommand<BsonDocument>("{ dbStats: 1}");
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

        public static long GetSize(string connection, string databaseName)
        {
            var stats = GetDbStats(connection, databaseName);
            var size = (decimal) Convert.ChangeType(stats["dataSize"], typeof(decimal));
            return Convert.ToInt64(size);
        }
    }
}