using MongoDB.Bson;

namespace BlueGoat.MongoUtils.Test.Integration.TestModels
{
    public class ToDo
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime DueBy { get; set; }
        public DateTime CreatedOn { get; internal set; }
        public DateTime? CompletedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? AssignedTo { get; set; }
        public Guid? CompletedBy { get; set; }
        public bool IsCancelled { get; set; }

        public bool IsCompleted => CompletedOn != null;
        

        public static ToDo CreateToDo(int sequence, User user)
        {
            var dueBy = DateTime.Now.AddDays(sequence);
            var isCompleted = sequence % 2 == 0;
            return new ToDo()
            {
                Id = Guid.NewGuid(),
                Title = $"ToDo #{sequence}",
                Description = $"This is the #{sequence} ToDo created for this test",
                DueBy = dueBy,
                CreatedOn = dueBy - TimeSpan.FromDays(7),
                CompletedOn = isCompleted ? dueBy - TimeSpan.FromDays(1) : null,
                CreatedBy = user.Id,
                AssignedTo = user.Id,
                CompletedBy = isCompleted ? user.Id : null,
                IsCancelled = false
            };
        }
    }
}
