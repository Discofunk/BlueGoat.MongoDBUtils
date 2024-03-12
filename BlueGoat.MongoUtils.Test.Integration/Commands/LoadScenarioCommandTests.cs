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
    public class LoadScenarioCommandTests : IDisposable
    {
        private readonly IMongoClient client;
        private readonly string connectionString;
        private readonly MongoUtilsRootCommand rootCommand;
        private readonly string loadScenarioFilePath = "LoadScenarioData.json";

        public LoadScenarioCommandTests(MongoDbTestContext context, ITestOutputHelper output)
        {
            client = context.Client;
            connectionString = context.ConnectionString;
            rootCommand = context.GetRootCommand(output);
        }

        [Fact]
        public async Task Can_load_scenario_data_from_file()
        {
            //Arrange
            var recordCount = 5;
            var databaseName = $"{nameof(LoadScenarioCommandTests)}_{nameof(Can_load_scenario_data_from_file)}";
            var store = new TestStore(client, databaseName);
            await store.SaveData(recordCount, loadScenarioFilePath);
            var db = client.GetDatabase(databaseName);
            var toDoCollection = db.GetCollection<ToDo>(nameof(ToDo));
            var userCollection = db.GetCollection<User>(nameof(User));

            File.Exists(loadScenarioFilePath).Should().BeTrue();

            //Act
            await rootCommand.InvokeAsync($"scenario load -c {connectionString} -db {databaseName} -in {loadScenarioFilePath}");

            //Assert
            var toDoCount = await toDoCollection.CountDocumentsAsync(FilterDefinition<ToDo>.Empty);
            var userCount = await userCollection.CountDocumentsAsync(FilterDefinition<User>.Empty);

            toDoCount.Should().Be(recordCount);
            userCount.Should().Be(recordCount);
        }

        public void Dispose()
        {
            File.Delete(loadScenarioFilePath);
        }
    }
}
