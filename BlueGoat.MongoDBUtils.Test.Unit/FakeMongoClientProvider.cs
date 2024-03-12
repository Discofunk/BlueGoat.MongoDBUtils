using MongoDB.Driver;

namespace BlueGoat.MongoDBUtils.Test.Unit
{
    internal class FakeMongoClientProvider : IMongoClientProvider
    {
        private readonly IMongoClient client;

        public FakeMongoClientProvider(IMongoClient client)
        {
            this.client = client;
        }

        public IMongoClient GetClient(string connectionString)
        {
            return client;
        }
    }
}
