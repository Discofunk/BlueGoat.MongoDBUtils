using System;

namespace BlueGoat.MongoDBUtils.Test.Unit.TestModels
{
    public class ToDo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
