using BlueGoat.MongoDBUtils;
using BlueGoat.MongoDBUtils.Commands;
using MongoDB.Driver;
using Testcontainers.MongoDb;
using Xunit;
using Xunit.Abstractions;

namespace BlueGoat.MongoUtils.Test.Integration
{
    public class MongoDbTestContext : IAsyncLifetime
    {
        private readonly MongoDbContainer mongoDbContainer =
            new MongoDbBuilder().Build();

        public string ConnectionString => mongoDbContainer.GetConnectionString();
        public IMongoClient Client => new MongoClient(ConnectionString);

        public TestConsole? Console { get; private set; }

        public MongoUtilsRootCommand GetRootCommand(ITestOutputHelper output)
        {
            Console = new TestConsole(output);
            return new MongoUtilsRootCommand(new MongoClientProvider(), new MigrationRunner(), Console);
        }

        public Task InitializeAsync() => mongoDbContainer.StartAsync();

        public Task DisposeAsync() => mongoDbContainer.DisposeAsync().AsTask();
    }

    [CollectionDefinition(Name)]
    public class MongoDbCollection : ICollectionFixture<MongoDbTestContext>
    {
        public const string Name = "MongoDB Collection";
    }
}