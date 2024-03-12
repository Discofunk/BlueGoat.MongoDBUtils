namespace BlueGoat.MongoUtils.Test.Integration.TestModels
{
    public class User
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        public static User CreateFromFullName(string fullName)
        {
            var names = fullName.Split(' ');
            return new User
            {
                Id = Guid.NewGuid(),
                FirstName = names[0],
                LastName = names[1],
                Email = $"{names[0]}.{names[1]}@example.com"
            };
        }
    }
}
