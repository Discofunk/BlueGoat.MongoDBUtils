using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace BlueGoat.MongoUtils.Test.Integration.TestModels
{
    public class TestStore
    {
        private readonly IMongoClient client;
        private readonly Lazy<IMongoCollection<ToDo>> toDoCollection;
        private readonly Lazy<IMongoCollection<User>> userCollection;

        public TestStore(IMongoClient client, string databaseName)
        {
            this.client = client;
            toDoCollection = new Lazy<IMongoCollection<ToDo>>(() =>
            {
                var db = client.GetDatabase(databaseName);
                return db.GetCollection<ToDo>(nameof(ToDo));
            });
            userCollection = new Lazy<IMongoCollection<User>>(() =>
            {
                var db = client.GetDatabase(databaseName);
                return db.GetCollection<User>(nameof(User));
            });
        }

        public async Task InsertDataAsync(int count)
        {
            var users = Enumerable.Range(0, count).Select(i => User.CreateFromFullName(Names.GetRandomName())).ToArray();
            await userCollection.Value.InsertManyAsync(users);
            var toDos = Enumerable.Range(0, count).Select(i => ToDo.CreateToDo(i, users[i]));
            await toDoCollection.Value.InsertManyAsync(toDos);
        }

        public async Task SaveDataAsync(int count, string filePath)
        {
            var users = Enumerable.Range(0, count).Select(i => User.CreateFromFullName(Names.GetRandomName())).ToArray();
            var toDos = Enumerable.Range(0, count).Select(i => ToDo.CreateToDo(i, users[i])).ToArray();

            var root = new BsonDocument
            {
                new BsonElement(nameof(ToDo), new BsonArray(toDos.Select(x => x.ToBsonDocument()))),
                new BsonElement(nameof(User), new BsonArray(users.Select(x => x.ToBsonDocument())))
            };
            var fileInfo = new FileInfo(filePath);
            fileInfo.Directory?.Create();
            var json = root.ToJson(new JsonWriterSettings() { Indent = true, OutputMode = JsonOutputMode.Shell });
            
            await using var fileStream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write);
            await using var sw = new StreamWriter(fileStream);
            await sw.WriteLineAsync(json);
        }
    }
}
