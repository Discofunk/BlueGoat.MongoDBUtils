using BlueGoat.MongoUtils.Test.Integration.TestModels;
using MongoDB.Driver;
using MongoDBMigrations;
using Version = MongoDBMigrations.Version;

namespace BlueGoat.MongoUtils.Test.Integration.TestMigrations
{
    public class V1_0_0_CreateToDoCollection : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            database.CreateCollection(nameof(ToDo));
        }

        public void Down(IMongoDatabase database)
        {
            database.DropCollection(nameof(ToDo));
        }

        public Version Version => new Version(1, 0, 0);
        public string Name => nameof(V1_0_0_CreateToDoCollection);
    }
}
