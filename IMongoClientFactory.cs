using MongoDB.Driver;

namespace BlueGoat.MongoDBUtils
{
    public interface IMongoClientFactory
    {
        IMongoClient GetClient(string connectionString);
    }

    public class MongoClientFactory : IMongoClientFactory
    {
        public IMongoClient GetClient(string connectionString)
        {
            return new MongoClient(connectionString);
        }
    }
}
