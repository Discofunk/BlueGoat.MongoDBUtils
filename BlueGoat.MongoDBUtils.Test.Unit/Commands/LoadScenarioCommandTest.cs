using BlueGoat.MongoDBUtils.Commands;
using MongoDB.Driver;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlueGoat.MongoDBUtils.Test.Unit.TestModels;
using Xunit;
using Xunit.Abstractions;
using System.CommandLine;
using System.CommandLine.Parsing;
using FluentAssertions;
using MongoDB.Bson;

namespace BlueGoat.MongoDBUtils.Test.Unit.Commands
{
    public class LoadScenarioCommandTest : IDisposable
    {
        private readonly MongoUtilsRootCommand rootCommand;
        private readonly TestConsole console;
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;
        private readonly string loadScenarioFilePath = "LoadScenarioFile.json";
        private readonly string databaseName = "MyMongoDb";
        private readonly IMongoCollection<BsonDocument> collection;
        private readonly string collectionName = nameof(ToDo);

        public LoadScenarioCommandTest(ITestOutputHelper output)
        {
            var migrationRunner = Substitute.For<IMigrationRunner>();
            client = Substitute.For<IMongoClient>();
            database = Substitute.For<IMongoDatabase>();
            console = new TestConsole(output);
            rootCommand = new MongoUtilsRootCommand(new FakeMongoClientFactory(client), migrationRunner, console);
            collection = Substitute.For<IMongoCollection<BsonDocument>>();
        }

        [Fact]
        public void Loads_Scenario_From_File()
        {
            //Arrange
            var numberOfRecords = 1000;
            var data = ToDo.CreateToDoScenarioData(numberOfRecords);
            File.WriteAllText(loadScenarioFilePath, data.ToJson());

            client.GetDatabase(databaseName).Returns(database);
            database.GetCollection<BsonDocument>(collectionName).Returns(collection);
            collection.EstimatedDocumentCount().Returns(0);
            
            var expectedOutput = new[]
            {
                $"Loaded {numberOfRecords} into \"ToDo\"",
                $"Scenario Load Completed"
            };

            //Act
            rootCommand.Parse($"scenario load -c mongodb://user:password@localhost:27017 -db {databaseName} -in {loadScenarioFilePath}").Invoke();

            //Assert
            database.Received(1).GetCollection<BsonDocument>(collectionName);
            collection.Received(1).EstimatedDocumentCount();
            collection.Received(1).DeleteMany(Arg.Any<FilterDefinition<BsonDocument>>());
            collection.Received(1).InsertMany(Arg.Any<List<BsonDocument>>());
            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        [Theory]
        [InlineData("Y")]
        [InlineData("A")]
        public void Prompts_user_and_deletes_existing_data_when_collection_has_data_and_user_enters_Y_or_A(string promptResponse)
        {
            //Arrange
            var numberOfRecords = 1000;
            var data = ToDo.CreateToDoScenarioData(numberOfRecords);
            File.WriteAllText(loadScenarioFilePath, data.ToJson());

            client.GetDatabase(databaseName).Returns(database);
            database.GetCollection<BsonDocument>(nameof(ToDo)).Returns(collection);
            collection.EstimatedDocumentCount().Returns(100);

            console.AddNextInput(promptResponse);

            var expectedOutput = new[]
            {
                $"Collection \"{collectionName}\" contains existing data. Delete existing data first? [Y]es / [N]o / [A]ll / [C]ancel: ",
                $"Loaded {numberOfRecords} into \"ToDo\"",
                $"Scenario Load Completed"
            };

            //Act
            rootCommand.Parse($"scenario load -c mongodb://user:password@localhost:27017 -db {databaseName} -in {loadScenarioFilePath}").Invoke();

            //Assert
            database.Received(1).GetCollection<BsonDocument>(nameof(ToDo));
            collection.Received(1).EstimatedDocumentCount();
            collection.Received(1).DeleteMany(Arg.Any<FilterDefinition<BsonDocument>>());
            collection.Received(1).InsertMany(Arg.Any<List<BsonDocument>>());
            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void Deletes_existing_data_when_collection_has_data_with_force_option()
        {
            //Arrange
            var numberOfRecords = 1000;
            var data = ToDo.CreateToDoScenarioData(numberOfRecords);
            File.WriteAllText(loadScenarioFilePath, data.ToJson());

            client.GetDatabase(databaseName).Returns(database);
            database.GetCollection<BsonDocument>(nameof(ToDo)).Returns(collection);
            collection.EstimatedDocumentCount().Returns(100);

            var expectedOutput = new[]
            {
                $"Loaded {numberOfRecords} into \"ToDo\"",
                $"Scenario Load Completed"
            };

            //Act
            rootCommand.Parse($"scenario load -c mongodb://user:password@localhost:27017 -db {databaseName} -in {loadScenarioFilePath} --force").Invoke();

            //Assert
            database.Received(1).GetCollection<BsonDocument>(nameof(ToDo));
            collection.DidNotReceive().EstimatedDocumentCount();
            collection.Received(1).DeleteMany(Arg.Any<FilterDefinition<BsonDocument>>());
            collection.Received(1).InsertMany(Arg.Any<List<BsonDocument>>());
            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void Prompts_user_and_does_not_delete_existing_data_or_load_data_when_collection_has_data_and_user_cancels()
        {
            //Arrange
            var numberOfRecords = 1000;
            var data = ToDo.CreateToDoScenarioData(numberOfRecords);
            File.WriteAllText(loadScenarioFilePath, data.ToJson());

            client.GetDatabase(databaseName).Returns(database);
            database.GetCollection<BsonDocument>(nameof(ToDo)).Returns(collection);
            collection.EstimatedDocumentCount().Returns(100);

            console.AddNextInput("N");

            var expectedOutput = new[]
            {
                $"Collection \"{collectionName}\" contains existing data. Delete existing data first? [Y]es / [N]o / [A]ll / [C]ancel: ",
                $"Scenario Load Completed"
            };

            //Act
            rootCommand.Parse($"scenario load -c mongodb://user:password@localhost:27017 -db {databaseName} -in {loadScenarioFilePath}").Invoke();

            //Assert
            database.Received(1).GetCollection<BsonDocument>(nameof(ToDo));
            collection.Received(1).EstimatedDocumentCount();
            collection.DidNotReceive().DeleteMany(Arg.Any<FilterDefinition<BsonDocument>>());
            collection.DidNotReceive().InsertMany(Arg.Any<List<BsonDocument>>());
            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void Does_not_load_scenario_data_when_file_not_found()
        {
            //Arrange
            var expectedOutput = new[]
            {
                $"File {loadScenarioFilePath} does not exist"
            };

            //Act
            rootCommand.Parse($"scenario load -c mongodb://user:password@localhost:27017 -db {databaseName} -in {loadScenarioFilePath}").Invoke();

            //Assert
            database.DidNotReceive().GetCollection<BsonDocument>(nameof(ToDo));
            collection.DidNotReceive().EstimatedDocumentCount();
            collection.DidNotReceive().DeleteMany(Arg.Any<FilterDefinition<BsonDocument>>());
            collection.DidNotReceive().InsertMany(Arg.Any<List<BsonDocument>>());

            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void User_is_prompted_to_continue_when_file_size_is_larger_than_threshold_and_does_not_continue_if_user_cancels()
        {
            //Arrange
            var numberOfRecords = 100000;
            var data = ToDo.CreateToDoScenarioData(numberOfRecords);
            File.WriteAllText(loadScenarioFilePath, data.ToJson());
            
            console.AddNextInput("N");

            var expectedOutput = new[]
            {
                $"The selected file is larger than {Parameters.LargeFileSizeWarningThresholdInMegaBytes}MB and may result in long loading time or timeouts.  Continue? [Y]es / [N]o: "
            };

            //Act
            rootCommand.Parse($"scenario load -c mongodb://user:password@localhost:27017 -db {databaseName} -in {loadScenarioFilePath}").Invoke();

            //Assert
            database.DidNotReceive().GetCollection<BsonDocument>(nameof(ToDo));
            collection.DidNotReceive().EstimatedDocumentCount();
            collection.DidNotReceive().DeleteMany(Arg.Any<FilterDefinition<BsonDocument>>());
            collection.DidNotReceive().InsertMany(Arg.Any<List<BsonDocument>>());
            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void User_is_prompted_to_continue_when_file_size_is_larger_than_threshold_and_continues_is_user_response_is_Y()
        {
            //Arrange
            var numberOfRecords = 100000;
            var data = ToDo.CreateToDoScenarioData(numberOfRecords);
            File.WriteAllText(loadScenarioFilePath, data.ToJson());

            client.GetDatabase(databaseName).Returns(database);
            database.GetCollection<BsonDocument>(nameof(ToDo)).Returns(collection);
            collection.EstimatedDocumentCount().Returns(0);

            console.AddNextInput("Y");

            var expectedOutput = new[]
            {
                $"The selected file is larger than {Parameters.LargeFileSizeWarningThresholdInMegaBytes}MB and may result in long loading time or timeouts.  Continue? [Y]es / [N]o: ",
                $"Loaded {numberOfRecords} into \"ToDo\"",
                "Scenario Load Completed"
            };

            //Act
            rootCommand.Parse($"scenario load -c mongodb://user:password@localhost:27017 -db {databaseName} -in {loadScenarioFilePath}").Invoke();

            //Assert
            database.Received(1).GetCollection<BsonDocument>(nameof(ToDo));
            collection.Received(1).EstimatedDocumentCount();
            collection.Received(1).DeleteMany(Arg.Any<FilterDefinition<BsonDocument>>());
            collection.Received(1).InsertMany(Arg.Any<List<BsonDocument>>());
            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        public void Dispose()
        {
            File.Delete(loadScenarioFilePath);
        }
    }
}
