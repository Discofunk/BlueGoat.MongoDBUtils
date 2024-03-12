using MongoDB.Bson;

namespace BlueGoat.MongoUtils.Test.Integration.TestModels
{
    public class ToDo
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public Guid AssignedToUserId { get; set; }

        public static ToDo CreateToDo(int sequence, User assignedTo)
        {
            return new ToDo()
            {
                Id = Guid.NewGuid(),
                Name = $"ToDo #{sequence}",
                DueDate = DateTime.Now.AddDays(sequence),
                IsCompleted = sequence % 2 == 0,
                AssignedToUserId = assignedTo.Id
            };
        }
    }
}
