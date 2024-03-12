using BlueGoat.MongoDBUtils.Commands;
using MongoDB.Driver;
using NSubstitute;
using System.CommandLine;
using System.CommandLine.Parsing;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace BlueGoat.MongoDBUtils.Test.Unit.Commands
{
    public class DropDatabaseCommandTests
    {
        private readonly MongoUtilsRootCommand rootCommand;
        private readonly TestConsole console;
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;
        private readonly string databaseName = "MyMongoDb";

        public DropDatabaseCommandTests(ITestOutputHelper output)
        {
            var migrationRunner = Substitute.For<IMigrationRunner>();
            client = Substitute.For<IMongoClient>();
            database = Substitute.For<IMongoDatabase>();
            console = new TestConsole(output);
            rootCommand = new MongoUtilsRootCommand(new FakeMongoClientFactory(client), migrationRunner, console);
        }

        [Fact]
        public void Database_is_dropped_when_forced()
        {
            //Arrange
            client.GetDatabase(databaseName).Returns(database);

            //Act
            rootCommand.Parse($"drop -c mongodb://user:password@localhost:27017 -db {databaseName} --force").Invoke();

            //Assert
            console.Outputs.Should()
                .Contain($"Database {databaseName} dropped");
            client.Received(1).DropDatabase(databaseName);
        }

        [Fact]
        public void Database_is_dropped_when_user_continues()
        {
            //Arrange
            client.GetDatabase(databaseName).Returns(database);
            console.AddNextInput("Y");

            //Act
            rootCommand.Parse($"drop -c mongodb://user:password@localhost:27017 -db {databaseName}").Invoke();

            //Assert
            console.Outputs.Should()
                .Contain($"Database {databaseName} dropped").And
                .Contain("Dropping database will permanently delete all data. Are you sure you want to continue? [Y]es / [N]o: ");
            client.Received(1).DropDatabase(databaseName);
        }

        [Fact]
        public void Database_is_not_dropped_when_user_does_not_continue()
        {
            //Arrange
            client.GetDatabase(databaseName).Returns(database);
            console.AddNextInput("N");

            //Act
            rootCommand.Parse($"drop -c mongodb://user:password@localhost:27017 -db {databaseName}").Invoke();

            //Assert
            console.Outputs.Should()
                .Contain("Dropping database will permanently delete all data. Are you sure you want to continue? [Y]es / [N]o: ");
            client.DidNotReceive().DropDatabase(databaseName);
        }
    }
}
