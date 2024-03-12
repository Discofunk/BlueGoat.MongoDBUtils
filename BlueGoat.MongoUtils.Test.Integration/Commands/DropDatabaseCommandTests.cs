using BlueGoat.MongoDBUtils.Commands;
using BlueGoat.MongoUtils.Test.Integration.TestModels;
using FluentAssertions;
using MongoDB.Driver;
using System.CommandLine;
using Xunit;
using Xunit.Abstractions;

namespace BlueGoat.MongoUtils.Test.Integration.Commands
{
    [Collection(MongoDbCollection.Name)]
    public class DropDatabaseCommandTests
    {
        private readonly ITestOutputHelper output;
        private readonly IMongoClient client;
        private readonly string connectionString;
        private readonly MongoUtilsRootCommand rootCommand;

        public DropDatabaseCommandTests(MongoDbTestContext context, ITestOutputHelper output)
        {
            this.output = output;
            client = context.Client;
            connectionString = context.ConnectionString;
            rootCommand = context.GetRootCommand(output);
        }

        [Fact]
        public async Task Can_drop_database()
        {
            //Arrange
            var recordCount = 5;
            var databaseName = $"{nameof(DropDatabaseCommandTests)}_{nameof(Can_drop_database)}";
            var store = new TestStore(client, databaseName);
            await store.InsertDataAsync(recordCount);
            var db = client.GetDatabase(databaseName);
            var toDoCollection = db.GetCollection<ToDo>(nameof(ToDo));
            var userCollection = db.GetCollection<User>(nameof(User));

            var toDoCount = await toDoCollection.CountDocumentsAsync(FilterDefinition<ToDo>.Empty);
            var userCount = await userCollection.CountDocumentsAsync(FilterDefinition<User>.Empty);

            toDoCount.Should().Be(recordCount);
            userCount.Should().Be(recordCount);

            //Act
            var time = await TimeAction.ExecuteAsync(async () =>
            {
                await rootCommand.InvokeAsync($"drop -c {connectionString} -db {databaseName} --force");
            });

            output.WriteLine("Time: " + time);

            //Assert
            var dbs = await client.ListDatabaseNames().ToListAsync();
            dbs.Should().NotContain(databaseName);
        }
    }
}
