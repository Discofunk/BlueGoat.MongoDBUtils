using BlueGoat.MongoUtils.Test.Integration.TestModels;
using MongoDB.Driver;
using MongoDBMigrations;
using Version = MongoDBMigrations.Version;

namespace BlueGoat.MongoUtils.Test.Integration.TestMigrations
{
    internal class V1_0_2_ToDoIndexes : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            var collection = database.GetCollection<ToDo>(nameof(ToDo));

            var indexes = new[]
            {
                new CreateIndexModel<ToDo>(Builders<ToDo>.IndexKeys
                    .Ascending(x => x.AssignedTo)
                    .Ascending(x => x.DueBy)),
                new CreateIndexModel<ToDo>(Builders<ToDo>.IndexKeys
                    .Ascending(x => x.CompletedOn))
            };

            collection.Indexes.CreateMany(indexes);
        }

        public void Down(IMongoDatabase database)
        {
            var collection = database.GetCollection<ToDo>(nameof(ToDo));
            collection.Indexes.DropAll();
        }

        public Version Version => new Version(1, 0, 2);
        public string Name => nameof(V1_0_2_ToDoIndexes);
    }
}