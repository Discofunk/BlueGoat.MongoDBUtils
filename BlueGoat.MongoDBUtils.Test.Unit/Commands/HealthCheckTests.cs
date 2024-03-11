using BlueGoat.MongoDBUtils.Commands;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using System.CommandLine;
using System.CommandLine.Parsing;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace BlueGoat.MongoDBUtils.Test.Unit.Commands
{
    public class HealthCheckTests
    {
        private readonly MongoUtilsRootCommand rootCommand;
        private readonly TestConsole console;
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;
        private readonly string databaseName = "MyMongoDb";

        public HealthCheckTests(ITestOutputHelper output)
        {
            var migrationRunner = Substitute.For<IMigrationRunner>();
            client = Substitute.For<IMongoClient>();
            database = Substitute.For<IMongoDatabase>();
            console = new TestConsole(output);
            rootCommand = new MongoUtilsRootCommand(new FakeMongoClientFactory(client), migrationRunner, console);
        }

        [Fact]
        public void HealthCheck_command_calls_DbStats()
        {
            //Arrange
            client.GetDatabase(databaseName).Returns(database);
            var healthCheckMessage = """
                {
                  "Message" : "Hello Health Check"
                }
                """.ReplaceLineEndings();
            database.RunCommand(HealthService.DbHealthCheckCommand).Returns(BsonDocument.Parse(healthCheckMessage));
            var expectedOutput = new[]
            {
                "Connected!",
                healthCheckMessage.ReplaceLineEndings()
            };

            //Act
            rootCommand.Parse($"health -c mongodb://user:password@localhost:27017 -db {databaseName}").Invoke();

            //Assert
            database.Received(1).RunCommand(HealthService.DbHealthCheckCommand);
            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }
    }
}
