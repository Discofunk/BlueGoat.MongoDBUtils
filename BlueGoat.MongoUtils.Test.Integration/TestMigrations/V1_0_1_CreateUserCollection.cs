using BlueGoat.MongoUtils.Test.Integration.TestModels;
using MongoDB.Driver;
using MongoDBMigrations;
using Version = MongoDBMigrations.Version;

namespace BlueGoat.MongoUtils.Test.Integration.TestMigrations
{
    public class V1_0_1_CreateUserCollection : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            database.CreateCollection(nameof(User));
        }

        public void Down(IMongoDatabase database)
        {
            database.DropCollection(nameof(User));
        }

        public Version Version => new Version(1, 0, 1);
        public string Name => nameof(V1_0_1_CreateUserCollection);
    }
}
