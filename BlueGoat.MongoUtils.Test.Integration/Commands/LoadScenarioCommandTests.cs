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
        private readonly ITestOutputHelper output;
        private readonly IMongoClient client;
        private readonly string connectionString;
        private readonly MongoUtilsRootCommand rootCommand;
        private readonly string loadScenarioFileName;

        public LoadScenarioCommandTests(MongoDbTestContext context, ITestOutputHelper output)
        {
            this.output = output;
            client = context.Client;
            connectionString = context.ConnectionString;
            rootCommand = context.GetRootCommand(output);
            loadScenarioFileName = "Load_" + Path.GetRandomFileName() + ".json";
        }

        [Fact]
        public async Task Can_load_scenario_data_from_file()
        {
            //Arrange
            var recordCount = 5;
            var databaseName = $"{nameof(LoadScenarioCommandTests)}_{nameof(Can_load_scenario_data_from_file)}";
            var store = new TestStore(client, databaseName);
            await store.SaveDataAsync(recordCount, loadScenarioFileName);
            var db = client.GetDatabase(databaseName);
            var toDoCollection = db.GetCollection<ToDo>(nameof(ToDo));
            var userCollection = db.GetCollection<User>(nameof(User));

            File.Exists(loadScenarioFileName).Should().BeTrue();

            //Act
            var time = await TimeAction.ExecuteAsync(async () =>
            {
                await rootCommand.InvokeAsync($"scenario load -c {connectionString} -db {databaseName} -in {loadScenarioFileName}");
            });
            
            output.WriteLine("Time: " + time);

            //Assert
            var toDoCount = await toDoCollection.CountDocumentsAsync(FilterDefinition<ToDo>.Empty);
            var userCount = await userCollection.CountDocumentsAsync(FilterDefinition<User>.Empty);

            toDoCount.Should().Be(recordCount);
            userCount.Should().Be(recordCount);
        }

        [Fact]
        public async Task Can_load_large_scenario_data_from_file()
        {
            //Arrange
            var recordCount = 100000;
            var databaseName = $"{nameof(LoadScenarioCommandTests)}_{nameof(Can_load_large_scenario_data_from_file)}";
            var store = new TestStore(client, databaseName);
            await store.SaveDataAsync(recordCount, loadScenarioFileName);
            var db = client.GetDatabase(databaseName);
            var toDoCollection = db.GetCollection<ToDo>(nameof(ToDo));
            var userCollection = db.GetCollection<User>(nameof(User));

            File.Exists(loadScenarioFileName).Should().BeTrue();

            //Act
            var time = await TimeAction.ExecuteAsync(async () =>
            {
                await rootCommand.InvokeAsync($"scenario load -c {connectionString} -db {databaseName} -in {loadScenarioFileName} --force");
            });

            output.WriteLine("Time: " + time);


            //Assert
            var toDoCount = await toDoCollection.CountDocumentsAsync(FilterDefinition<ToDo>.Empty);
            var userCount = await userCollection.CountDocumentsAsync(FilterDefinition<User>.Empty);

            toDoCount.Should().Be(recordCount);
            userCount.Should().Be(recordCount);
        }

        public void Dispose()
        {
            File.Delete(loadScenarioFileName);
        }
    }
}
