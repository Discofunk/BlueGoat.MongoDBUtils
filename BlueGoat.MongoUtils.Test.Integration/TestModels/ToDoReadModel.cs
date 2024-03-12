using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueGoat.MongoUtils.Test.Integration.TestModels
{
    public class ToDoReadModel
    {
        public Guid Id { get; set; }
        public string VersionHash { get; set; } = string.Empty;
        public long AggregateVersion { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime CompletedOn { get; set; }
        public ResourceReference<Guid>? AssignedTo { get; set; }
        public ResourceReference<Guid>? CompletedBy { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsAssigned { get; set; }
    }
}
