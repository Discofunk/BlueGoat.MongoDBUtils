using BlueGoat.MongoDBUtils.Commands;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using MongoDB.Driver;
using System.CommandLine;
using System.Reflection;
using BlueGoat.MongoUtils.Test.Integration.TestModels;
using FluentAssertions;
using MongoDB.Bson;
using Xunit;
using Xunit.Abstractions;

namespace BlueGoat.MongoUtils.Test.Integration.Commands
{
    [Collection(MongoDbCollection.Name)]
    public class MigrationCommandTests
    {
        private readonly ITestOutputHelper output;
        private readonly IMongoClient client;
        private readonly string connectionString;
        private readonly MongoUtilsRootCommand rootCommand;

        public MigrationCommandTests(MongoDbTestContext context, ITestOutputHelper output)
        {
            this.output = output;
            client = context.Client;
            connectionString = context.ConnectionString;
            rootCommand = context.GetRootCommand(output);
        }

        [Fact]
        public async Task Can_run_db_migrations()
        {
            //Arrange
            var databaseName = $"{nameof(MigrationCommandTests)}_{nameof(Can_run_db_migrations)}";
            var db = client.GetDatabase(databaseName);
            var migrationAssembly = Assembly.GetExecutingAssembly().Location;

            var expectedCollections = new[]
            {
                nameof(ToDo),
                nameof(User),
                nameof(ToDoReadModel)
            };

            //Act
            var time = await TimeAction.ExecuteAsync(async () =>
            {
                await rootCommand.InvokeAsync($"migrate -c {connectionString} -db {databaseName} -ma {migrationAssembly}");
            });

            output.WriteLine("Time: " + time);

            //Assert
            var collections = await db.ListCollectionNames().ToListAsync();
            collections.Should().Contain(expectedCollections);
            var toDoCollection = db.GetCollection<ToDo>(nameof(ToDo));
            var indexes = await toDoCollection.Indexes.List().ToListAsync();
            indexes.Should().HaveCount(3);
            var migrationCollection =  db.GetCollection<BsonDocument>("_migrations");
            var migrationCount = await migrationCollection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty);
            migrationCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Can_rollback_migration()
        {
            //Arrange
            var databaseName = $"{nameof(MigrationCommandTests)}_{nameof(Can_run_db_migrations)}";
            var db = client.GetDatabase(databaseName);
            var migrationAssembly = Assembly.GetExecutingAssembly().Location;

            var expectedCollections = new[]
            {
                nameof(ToDo),
                nameof(User)
            };

            //Act
            var time = await TimeAction.ExecuteAsync(async () =>
            {
                await rootCommand.InvokeAsync($"migrate -c {connectionString} -db {databaseName} -ma {migrationAssembly}");
            });

            output.WriteLine("Time: " + time);

            time = await TimeAction.ExecuteAsync(async () =>
            {
                await rootCommand.InvokeAsync($"migrate -c {connectionString} -db {databaseName} -ma {migrationAssembly} --version 1.0.2");
            });

            output.WriteLine("Rollback Time: " + time);

            //Assert
            var collections = await db.ListCollectionNames().ToListAsync();
            collections.Should().Contain(expectedCollections);
            var toDoCollection = db.GetCollection<ToDo>(nameof(ToDo));
            var indexes = await toDoCollection.Indexes.List().ToListAsync();
            indexes.Should().HaveCount(3);
            var migrationCollection = db.GetCollection<BsonDocument>("_migrations");
            var migrationCount = await migrationCollection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty);
            migrationCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Running_migration_again_does_not_change_schema()
        {
            //Arrange
            var databaseName = $"{nameof(MigrationCommandTests)}_{nameof(Can_run_db_migrations)}";
            var db = client.GetDatabase(databaseName);
            var migrationAssembly = Assembly.GetExecutingAssembly().Location;

            var expectedCollections = new[]
            {
                nameof(ToDo),
                nameof(User)
            };

            //Act
            var time = await TimeAction.ExecuteAsync(async () =>
            {
                await rootCommand.InvokeAsync($"migrate -c {connectionString} -db {databaseName} -ma {migrationAssembly}");
            });

            output.WriteLine("Time: " + time);

            time = await TimeAction.ExecuteAsync(async () =>
            {
                await rootCommand.InvokeAsync($"migrate -c {connectionString} -db {databaseName} -ma {migrationAssembly}");
            });

            output.WriteLine("Rollback Time: " + time);

            //Assert
            var collections = await db.ListCollectionNames().ToListAsync();
            collections.Should().Contain(expectedCollections);
            var toDoCollection = db.GetCollection<ToDo>(nameof(ToDo));
            var indexes = await toDoCollection.Indexes.List().ToListAsync();
            indexes.Should().HaveCount(3);
            var migrationCollection = db.GetCollection<BsonDocument>("_migrations");
            var migrationCount = await migrationCollection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty);
            migrationCount.Should().BeGreaterThan(0);
        }
    }
}
