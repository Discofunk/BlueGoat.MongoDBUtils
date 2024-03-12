﻿using System.CommandLine;
using BlueGoat.MongoDBUtils.Commands;
using BlueGoat.MongoUtils.Test.Integration.TestModels;
using FluentAssertions;
using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;

namespace BlueGoat.MongoUtils.Test.Integration.Commands
{
    [Collection(MongoDbCollection.Name)]
    public class SaveScenarioCommandTests : IDisposable
    {
        private readonly IMongoClient client;
        private readonly string connectionString;
        private readonly MongoUtilsRootCommand rootCommand;
        private readonly string savedScenarioFilePath = "SaveScenarioData.json";

        public SaveScenarioCommandTests(MongoDbTestContext context, ITestOutputHelper output)
        {
            client = context.Client;
            connectionString = context.ConnectionString;
            rootCommand = context.GetRootCommand(output);
        }

        [Fact]
        public async Task Can_save_scenario_data_to_file()
        {
            //Arrange
            var recordCount = 5;
            var databaseName = $"{nameof(SaveScenarioCommandTests)}_{nameof(Can_save_scenario_data_to_file)}";
            var store = new TestStore(client, databaseName);
            await store.InsertData(recordCount);
            var db = client.GetDatabase(databaseName);
            var toDoCollection = db.GetCollection<ToDo>(nameof(ToDo));
            var userCollection = db.GetCollection<User>(nameof(User));

            var toDoCount = await toDoCollection.CountDocumentsAsync(FilterDefinition<ToDo>.Empty);
            var userCount = await userCollection.CountDocumentsAsync(FilterDefinition<User>.Empty);
            
            toDoCount.Should().Be(recordCount);
            userCount.Should().Be(recordCount);

            //Act
            await rootCommand.InvokeAsync($"scenario save -c {connectionString} -db {databaseName} -out {savedScenarioFilePath}");

            //Assert
            var fileInfo = new FileInfo(savedScenarioFilePath);
            fileInfo.Exists.Should().BeTrue();
            fileInfo.Length.Should().BeGreaterThan(0);
        }

        public void Dispose()
        {
            File.Delete(savedScenarioFilePath);
        }
    }
}
