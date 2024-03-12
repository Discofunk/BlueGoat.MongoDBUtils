using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlueGoat.MongoDBUtils.Test.Unit.TestModels
{
    public class ToDo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        public static BsonDocument CreateToDoScenarioData(int size)
        {
            var root = new BsonDocument();
            var data = CreateData(size);
            var array = new BsonArray(data);
            var e = new BsonElement("ToDo", array);
            root.Add(e);
            return root;
        }

        public static IEnumerable<BsonDocument> CreateData(int size)
        {
            var data = Enumerable.Range(1, size).Select(x => new ToDo()
            {
                Id = Guid.NewGuid(),
                Name = $"ToDo #{x}",
                DueDate = DateTime.Now.AddDays(x),
                IsCompleted = x % 2 == 0
            }.ToBsonDocument());
            return data;
        }
    }
}
