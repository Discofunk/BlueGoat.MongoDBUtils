using MongoDB.Driver;

namespace BlueGoat.MongoDBUtils.Test.Unit
{
    internal class FakeMongoClientFactory : IMongoClientFactory
    {
        private readonly IMongoClient client;

        public FakeMongoClientFactory(IMongoClient client)
        {
            this.client = client;
        }

        public IMongoClient GetClient(string connectionString)
        {
            return client;
        }
    }
}
