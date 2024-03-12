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
    public class HealthCheckCommandTests
    {
        private readonly ITestOutputHelper output;
        private readonly IMongoClient client;
        private readonly string connectionString;
        private readonly MongoUtilsRootCommand rootCommand;
        private readonly TestConsole console;

        public HealthCheckCommandTests(MongoDbTestContext context, ITestOutputHelper output)
        {
            this.output = output;
            client = context.Client;
            connectionString = context.ConnectionString;
            rootCommand = context.GetRootCommand(output);
            console = context.Console;
        }

        [Fact]
        public async Task Should_return_health_info()
        {
            //Arrange
            var recordCount = 5;
            var databaseName = $"{nameof(HealthCheckCommandTests)}_{nameof(Should_return_health_info)}";
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
                await rootCommand.InvokeAsync($"health -c {connectionString} -db {databaseName}");
            });

            output.WriteLine("Time: " + time);

            //Assert
            console.Outputs.Should().HaveCount(2);
            var stats = console.Outputs[1];
            stats.Should().Contain($"\"db\" : \"{databaseName}\"");
        }
    }
}
