using BlueGoat.MongoUtils.Test.Integration.TestModels;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBMigrations;
using Version = MongoDBMigrations.Version;

namespace BlueGoat.MongoUtils.Test.Integration.TestMigrations
{
    public class V1_0_3_ToDoReadView : IMigration
    {
        public void Up(IMongoDatabase database)
        {
            var view = new[]
            {
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        {"from", "User"},
                        {"localField", "AssignedTo"},
                        {"foreignField", "_id"},
                        {"as", "assignedToJoin"}
                    }),
                new BsonDocument("$unwind",
                    new BsonDocument
                    {
                        {"path", "$assignedToJoin"},
                        {"preserveNullAndEmptyArrays", true}
                    }),
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        {"from", "User"},
                        {"localField", "CompletedBy"},
                        {"foreignField", "_id"},
                        {"as", "completedByJoin"}
                    }),
                new BsonDocument("$unwind",
                    new BsonDocument
                    {
                        {"path", "$completedByJoin"},
                        {"preserveNullAndEmptyArrays", true}
                    }),
                new BsonDocument("$project",
                    new BsonDocument
                    {
                        {"Title", "$Title"},
                        {"Description", "$Description"},
                        {"CreatedOn", "$CreatedOn"},
                        {
                            "IsAssigned",
                            new BsonDocument("$ne",
                                new BsonArray
                                {
                                    "$AssignedTo",
                                    BsonNull.Value
                                })
                        },
                        {"CompletedOn", "$CreatedOn"},
                        {"DueBy", "$DueBy"},
                        {"IsCancelled", "$IsCancelled"},
                        {"IsCompleted", "$IsCompleted"},
                        {
                            "CreatedBy",
                            new BsonDocument
                            {
                                {"ResourceId", "$createdByJoin._id"},
                                {"DisplayName", "$createdByJoin.FirstName"},
                            }
                        },
                        {
                            "AssignedTo",
                            new BsonDocument("$cond",
                                new BsonArray
                                {
                                    new BsonDocument("$ne",
                                        new BsonArray
                                        {
                                            "$AssignedTo",
                                            BsonNull.Value
                                        }),
                                    new BsonDocument
                                    {
                                        {"ResourceId", "$assignedToJoin._id"},
                                        {"DisplayName", "$assignedToJoin.FirstName"},
                                    },
                                    BsonNull.Value
                                })
                        },
                        {
                            "CompletedBy",
                            new BsonDocument("$cond",
                                new BsonArray
                                {
                                    new BsonDocument("$ne",
                                        new BsonArray
                                        {
                                            "$CompletedBy",
                                            BsonNull.Value
                                        }),
                                    new BsonDocument
                                    {
                                        {"ResourceId", "$completedByJoin._id"},
                                        {"DisplayName", "$completedByJoin.FirstName"},
                                    },
                                    BsonNull.Value
                                })
                        }
                    })
            };
            database.CreateView(nameof(ToDoReadModel), nameof(ToDo), PipelineDefinition<ToDo, ToDoReadModel>.Create(view));
        }

        public void Down(IMongoDatabase database)
        {
            database.DropCollection(nameof(ToDoReadModel));
        }

        public Version Version => new Version(1, 0, 3);
        public string Name => nameof(V1_0_3_ToDoReadView);
    }
}
