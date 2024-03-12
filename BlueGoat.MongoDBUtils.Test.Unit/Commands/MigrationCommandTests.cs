using BlueGoat.MongoDBUtils.Commands;
using MongoDB.Driver;
using MongoDBMigrations.Core;
using NSubstitute;
using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Reflection;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace BlueGoat.MongoDBUtils.Test.Unit.Commands
{
    public class MigrationCommandTests
    {
        private readonly IMigrationRunner migrationRunner;
        private readonly MongoUtilsRootCommand rootCommand;
        private readonly TestConsole console;
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;
        private readonly string databaseName = "MyMongoDb";

        public MigrationCommandTests(ITestOutputHelper output)
        {
            migrationRunner = Substitute.For<IMigrationRunner>();
            client = Substitute.For<IMongoClient>();
            database = Substitute.For<IMongoDatabase>();
            console = new TestConsole(output);
            rootCommand = new MongoUtilsRootCommand(new FakeMongoClientFactory(client), migrationRunner, console);
        }

        [Fact]
        public void Migration_is_run()
        {
            //Arrange
            var connection = "mongodb://user:password@localhost:27017";
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyPath = assembly.Location;

            var expectedOutput = new[]
            {
                "Migrations Run"
            };

            //Act
            rootCommand.Parse($"migrate -c {connection} -db {databaseName} -ma {assemblyPath}").Invoke();

            //Assert
            migrationRunner.Received(1).RunMigrations(assembly, connection, databaseName, null, Arg.Any<Action<InterimMigrationResult>>());
            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void Migration_is_run_for_target_version()
        {
            //Arrange
            var connection = "mongodb://user:password@localhost:27017";
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyPath = assembly.Location;
            var targetVersion = "2";

            var expectedOutput = new[]
            {
                "Migrations Run"
            };

            //Act
            rootCommand.Parse($"migrate -c {connection} -db {databaseName} -v {targetVersion} -ma {assemblyPath}").Invoke();

            //Assert
            migrationRunner.Received(1).RunMigrations(assembly, connection, databaseName, targetVersion, Arg.Any<Action<InterimMigrationResult>>());
            console.Outputs.Should().BeEquivalentTo(expectedOutput);
        }
    }
}
