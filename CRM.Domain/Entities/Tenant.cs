using CRM.Domain.Common;


namespace CRM.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public string Slug { get; private set; } = null!;
        public string Plan { get; private set; } = null!;
        public string Status { get; private set; } = null!;

        private readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users;

        private Tenant() { } //EF

        public Tenant(string name, string slug)
        {
            Id = Guid.NewGuid();
            Name = name;
            Slug = slug;
            Plan = "Free";
            Status = "Active";
            CreatedAtUtc = DateTime.UtcNow;

        }
    }
}
