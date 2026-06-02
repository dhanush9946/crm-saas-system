using Microsoft.EntityFrameworkCore.ChangeTracking;
using CRM.Shared.Audit;

namespace CRM.Infrastructure.Persistence.Interceptors;

public sealed class AuditEntry
{
    public EntityEntry Entry { get; }

    public string EntityType { get; set; } = default!;

    public string Action { get; set; } = default!;

    public string? EntityId { get; set; }

    public Dictionary<string, object?> NewValues { get; } = [];

    public List<PropertyChange> Changes { get; } = [];

    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }
}