using CRM.Domain.Common;
using CRM.Domain.Identity.Enums;

namespace CRM.Domain.Identity.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string Slug { get; private set; } = default!;
        public string Plan { get; private set; } = "Free";
        public TenantStatus Status { get; private set; } = TenantStatus.Active;

        private Tenant() { }

        private Tenant(string name, string slug)
        {
            SetName(name);
            SetSlug(slug);
            Plan = "Free";
            Status = TenantStatus.Active;
        }

        public static Tenant Create(string name, string slug)
        {
            return new Tenant(name, slug);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tenant name cannot be empty");

            Name = name;
            SetUpdated();
        }

        public void SetSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Slug cannot be empty");

            Slug = slug.ToLower().Trim();
            SetUpdated();
        }

        public void ChangePlan(string plan)
        {
            Plan = plan;
            SetUpdated();
        }

        public void Activate()
        {
            Status = TenantStatus.Active;
            SetUpdated();
        }

        public void Suspend()
        {
            Status = TenantStatus.Suspended;
            SetUpdated();
        }

 
    }
}
