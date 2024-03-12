using BlueGoat.MongoDBUtils.Commands;
using BlueGoat.MongoDBUtils.Test.Unit.TestModels;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace BlueGoat.MongoDBUtils.Test.Unit.Commands
{
    public class SaveScenarioCommandTests : IDisposable
    {
        private readonly MongoUtilsRootCommand rootCommand;
        private readonly TestConsole console;
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;
        private readonly string saveScenarioFilePath = "SaveScenarioFile.json";
        private readonly string databaseName = "MyMongoDb";
        private readonly IMongoCollection<BsonDocument> collection;
        private readonly string collectionName = nameof(ToDo);

        public SaveScenarioCommandTests(ITestOutputHelper output)
        {
            var migrationRunner = Substitute.For<IMigrationRunner>();
            client = Substitute.For<IMongoClient>();
            database = Substitute.For<IMongoDatabase>();
            console = new TestConsole(output);
            rootCommand = new MongoUtilsRootCommand(new FakeMongoClientFactory(client), migrationRunner, console);
            collection = Substitute.For<IMongoCollection<BsonDocument>>();
        }

        [Fact]
        public void Saves_scenario_data_to_file()
        {
            //Arrange
            var data = ToDo.CreateData(100).ToArray();

            var healthCheckMessage = """
                {
                  "db" : "MyMongoDb",
                  dataSize: 1000
                }
                """;
            database.RunCommand(HealthService.DbHealthCheckCommand).Returns(BsonDocument.Parse(healthCheckMessage));

            client.GetDatabase(databaseName).Returns(database);
            database.GetCollection<BsonDocument>(nameof(ToDo)).Returns(collection);
            database.ListCollectionNames(Arg.Any<ListCollectionNamesOptions>()).Returns(new TestAsyncCursor<string>(new []{collectionName}));
            database.GetCollection<BsonDocument>(collectionName).Returns(collection);
            collection.FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None)
                .Returns(new TestAsyncCursor<BsonDocument>(data));

            var expectedOutput = new[]
            {
                "Save Scenario Started",
                "Scenario Saved",
                saveScenarioFilePath
            };

            //Act
            rootCommand.Parse($"scenario save -c mongodb://user:password@localhost:27017 -db {databaseName} -out {saveScenarioFilePath}").Invoke();

            //Assert
            client.Received(2).GetDatabase(databaseName);
            database.Received(1).RunCommand(HealthService.DbHealthCheckCommand);
            database.Received(1).ListCollectionNames(Arg.Any<ListCollectionNamesOptions>());
            database.Received(1).GetCollection<BsonDocument>(collectionName);
            collection.Received(1).FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None);
            
            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void Prompts_user_when_file_already_exists_and_continues_when_user_enters_Y()
        {
            //Arrange
            var existingData = ToDo.CreateToDoScenarioData(100).ToArray();
            File.WriteAllText(saveScenarioFilePath, existingData.ToJson());

            console.AddNextInput("Y");

            var data = ToDo.CreateData(100).ToArray();
            var healthCheckMessage = """
                {
                  "db" : "MyMongoDb",
                  dataSize: 1000
                }
                """;

            database.RunCommand(HealthService.DbHealthCheckCommand).Returns(BsonDocument.Parse(healthCheckMessage));
            client.GetDatabase(databaseName).Returns(database);
            database.GetCollection<BsonDocument>(nameof(ToDo)).Returns(collection);
            database.ListCollectionNames(Arg.Any<ListCollectionNamesOptions>()).Returns(new TestAsyncCursor<string>(new[] { collectionName }));
            database.GetCollection<BsonDocument>(collectionName).Returns(collection);
            collection.FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None)
                .Returns(new TestAsyncCursor<BsonDocument>(data));

            var expectedOutput = new[]
            {
                "Save Scenario Started",
                $"File {saveScenarioFilePath} already exists. Overwrite? [Y]es /[N]o: ",
                "Scenario Saved",
                saveScenarioFilePath
            };

            //Act
            rootCommand.Parse($"scenario save -c mongodb://user:password@localhost:27017 -db {databaseName} -out {saveScenarioFilePath}").Invoke();

            //Assert
            client.Received(2).GetDatabase(databaseName);
            database.Received(1).RunCommand(HealthService.DbHealthCheckCommand);
            database.Received(1).ListCollectionNames(Arg.Any<ListCollectionNamesOptions>());
            database.Received(1).GetCollection<BsonDocument>(collectionName);
            collection.Received(1).FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None);

            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }
        [Fact]
        public void User_is_not_prompted_when_file_exists_with_force_option()
        {
            //Arrange
            var existingData = ToDo.CreateToDoScenarioData(100).ToArray();
            File.WriteAllText(saveScenarioFilePath, existingData.ToJson());

            console.AddNextInput("Y");

            var data = ToDo.CreateData(100).ToArray();
            var healthCheckMessage = """
                {
                  "db" : "MyMongoDb",
                  dataSize: 1000
                }
                """;

            database.RunCommand(HealthService.DbHealthCheckCommand).Returns(BsonDocument.Parse(healthCheckMessage));
            client.GetDatabase(databaseName).Returns(database);
            database.GetCollection<BsonDocument>(nameof(ToDo)).Returns(collection);
            database.ListCollectionNames(Arg.Any<ListCollectionNamesOptions>()).Returns(new TestAsyncCursor<string>(new[] { collectionName }));
            database.GetCollection<BsonDocument>(collectionName).Returns(collection);
            collection.FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None)
                .Returns(new TestAsyncCursor<BsonDocument>(data));

            var expectedOutput = new[]
            {
                "Save Scenario Started",
                $"File {saveScenarioFilePath} already exists. Overwrite? [Y]es /[N]o: ",
                "Scenario Saved",
                saveScenarioFilePath
            };

            //Act
            rootCommand.Parse($"scenario save -c mongodb://user:password@localhost:27017 -db {databaseName} -out {saveScenarioFilePath} --force").Invoke();

            //Assert
            client.Received(2).GetDatabase(databaseName);
            database.Received(1).RunCommand(HealthService.DbHealthCheckCommand);
            database.Received(1).ListCollectionNames(Arg.Any<ListCollectionNamesOptions>());
            database.Received(1).GetCollection<BsonDocument>(collectionName);
            collection.Received(1).FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None);

            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void Prompts_user_when_file_already_exists_and_does_not_save_when_user_enters_N()
        {
            //Arrange
            var existingData = ToDo.CreateToDoScenarioData(100).ToArray();
            File.WriteAllText(saveScenarioFilePath, existingData.ToJson());

            console.AddNextInput("N");

            var data = ToDo.CreateData(100).ToArray();
            var healthCheckMessage = """
                {
                  "db" : "MyMongoDb",
                  dataSize: 1000
                }
                """;

            database.RunCommand(HealthService.DbHealthCheckCommand).Returns(BsonDocument.Parse(healthCheckMessage));
            client.GetDatabase(databaseName).Returns(database);
            database.GetCollection<BsonDocument>(nameof(ToDo)).Returns(collection);
            database.ListCollectionNames(Arg.Any<ListCollectionNamesOptions>()).Returns(new TestAsyncCursor<string>(new[] { collectionName }));
            database.GetCollection<BsonDocument>(collectionName).Returns(collection);
            collection.FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None)
                .Returns(new TestAsyncCursor<BsonDocument>(data));

            var expectedOutput = new[]
            {
                "Save Scenario Started",
                $"File {saveScenarioFilePath} already exists. Overwrite? [Y]es /[N]o: "
            };

            //Act
            rootCommand.Parse($"scenario save -c mongodb://user:password@localhost:27017 -db {databaseName} -out {saveScenarioFilePath}").Invoke();

            //Assert
            client.Received(1).GetDatabase(databaseName);
            database.Received(1).RunCommand(HealthService.DbHealthCheckCommand);
            database.DidNotReceive().ListCollectionNames(Arg.Any<ListCollectionNamesOptions>());
            database.DidNotReceive().GetCollection<BsonDocument>(collectionName);
            collection.DidNotReceive().FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None);

            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void User_is_prompted_when_data_is_larger_than_threshold_and_continues_when_user_enters_Y()
        {
            //Arrange
            var numberOfRecords = 100000;
            var data = ToDo.CreateData(numberOfRecords).ToArray();

            console.AddNextInput("Y");

            var healthCheckMessage = """
                {
                  "db" : "MyMongoDb",
                  dataSize: 5000000
                }
                """;
            database.RunCommand(HealthService.DbHealthCheckCommand).Returns(BsonDocument.Parse(healthCheckMessage));

            client.GetDatabase(databaseName).Returns(database);
            database.GetCollection<BsonDocument>(nameof(ToDo)).Returns(collection);
            database.ListCollectionNames(Arg.Any<ListCollectionNamesOptions>()).Returns(new TestAsyncCursor<string>(new[] { collectionName }));
            database.GetCollection<BsonDocument>(collectionName).Returns(collection);
            collection.FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None)
                .Returns(new TestAsyncCursor<BsonDocument>(data));

            var expectedOutput = new[]
            {
                "Save Scenario Started",
                $"The database size is larger than {Parameters.LargeFileSizeWarningThresholdInMegaBytes}MB and may result in long save times or timeouts.  Continue? [Y]es / [N]o: ",
                "Scenario Saved",
                saveScenarioFilePath
            };

            //Act
            rootCommand.Parse($"scenario save -c mongodb://user:password@localhost:27017 -db {databaseName} -out {saveScenarioFilePath}").Invoke();

            //Assert
            client.Received(2).GetDatabase(databaseName);
            database.Received(1).RunCommand(HealthService.DbHealthCheckCommand);
            database.Received(1).ListCollectionNames(Arg.Any<ListCollectionNamesOptions>());
            database.Received(1).GetCollection<BsonDocument>(collectionName);
            collection.Received(1).FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None);

            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void User_is_prompted_when_data_is_larger_than_threshold_and_exits_when_user_enters_N()
        {
            //Arrange
            var numberOfRecords = 100000;
            var data = ToDo.CreateData(numberOfRecords).ToArray();

            console.AddNextInput("N");

            var healthCheckMessage = """
                {
                  "db" : "MyMongoDb",
                  dataSize: 5000000
                }
                """;
            database.RunCommand(HealthService.DbHealthCheckCommand).Returns(BsonDocument.Parse(healthCheckMessage));

            client.GetDatabase(databaseName).Returns(database);
            database.GetCollection<BsonDocument>(nameof(ToDo)).Returns(collection);
            database.ListCollectionNames(Arg.Any<ListCollectionNamesOptions>()).Returns(new TestAsyncCursor<string>(new[] { collectionName }));
            database.GetCollection<BsonDocument>(collectionName).Returns(collection);
            collection.FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None)
                .Returns(new TestAsyncCursor<BsonDocument>(data));

            var expectedOutput = new[]
            {
                "Save Scenario Started",
                $"The database size is larger than {Parameters.LargeFileSizeWarningThresholdInMegaBytes}MB and may result in long save times or timeouts.  Continue? [Y]es / [N]o: ",
            };

            //Act
            rootCommand.Parse($"scenario save -c mongodb://user:password@localhost:27017 -db {databaseName} -out {saveScenarioFilePath}").Invoke();

            //Assert
            client.Received(1).GetDatabase(databaseName);
            database.Received(1).RunCommand(HealthService.DbHealthCheckCommand);
            database.DidNotReceive().ListCollectionNames(Arg.Any<ListCollectionNamesOptions>());
            database.DidNotReceive().GetCollection<BsonDocument>(collectionName);
            collection.DidNotReceive().FindSync(Arg.Any<BsonDocumentFilterDefinition<BsonDocument>>(), Arg.Any<FindOptions<BsonDocument, BsonDocument>>(), CancellationToken.None);

            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        private class TestAsyncCursor<T> : IAsyncCursor<T>
        {
            private bool moved;

            public TestAsyncCursor(IEnumerable<T> current)
            {
                this.Current = current;
            }

            public void Dispose()
            {
            }

            public bool MoveNext(CancellationToken cancellationToken = new CancellationToken())
            {
                if (moved)
                {
                    return false;
                }

                moved = true;
                return true;
            }

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken = new CancellationToken())
            {
                return Task.FromResult(MoveNext(cancellationToken));
            }

            public IEnumerable<T> Current { get; }
        }

        public void Dispose()
        {
            File.Delete(saveScenarioFilePath);
        }
    }
}
