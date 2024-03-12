namespace BlueGoat.MongoUtils.Test.Integration.TestModels
{
    public class ResourceReference<TId> where TId : IComparable
    {
        public TId ResourceId { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
    }
}
