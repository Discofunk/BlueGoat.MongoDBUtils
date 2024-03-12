using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;

namespace BlueGoat.MongoUtils.Test.Integration
{
    public class MongoDbTestContext : IAsyncLifetime
    {
        private readonly MongoDbContainer mongoDbContainer =
            new MongoDbBuilder().Build();

        public string ConnectionString => mongoDbContainer.GetConnectionString();
        public IMongoClient Client => new MongoClient(ConnectionString);

        public Task InitializeAsync() => mongoDbContainer.StartAsync();

        public Task DisposeAsync() => mongoDbContainer.DisposeAsync().AsTask();
    }

    [CollectionDefinition(Name)]
    public class MongoDbCollection : ICollectionFixture<MongoDbTestContext>
    {
        public const string Name = "MongoDB Collection";
    }
}