using MongoDB.Driver;

namespace BlueGoat.MongoDBUtils
{
    public interface IMongoClientProvider
    {
        IMongoClient GetClient(string connectionString);
    }

    public class MongoClientProvider : IMongoClientProvider
    {
        public IMongoClient GetClient(string connectionString)
        {
            return new MongoClient(connectionString);
        }
    }
}
