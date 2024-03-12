using MongoDB.Driver;
using Xunit;

namespace BlueGoat.MongoUtils.Test.Integration
{
    [Collection(MongoDbCollection.Name)]
    public class DeleteMe
    {
        private readonly MongoDbTestContext mongoDbTestContext;

        public DeleteMe(MongoDbTestContext mongoDbTestContext)
        {
            this.mongoDbTestContext = mongoDbTestContext;
        }

        [Fact]
        public async Task CanConnect()
        {
            using var databases = await mongoDbTestContext.Client.ListDatabasesAsync();
            Assert.True(await databases.AnyAsync());
        }
    }
}
