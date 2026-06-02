using CRM.Application.Common.Interfaces;
using CRM.Domain.Common;
using CRM.Domain.Common.Constants;
using CRM.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using CRM.Shared.Audit;


namespace CRM.Infrastructure.Persistence.Interceptors;

public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser;

    public AuditInterceptor(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData,
    InterceptionResult<int> result,
    CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is not null)
        {
            var auditEntries = CreateAuditEntries(context);

            var auditLogs = CreateAuditLogs(auditEntries);

            context.Set<AuditLog>()
                   .AddRange(auditLogs);
        }

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }
    private List<AuditEntry> CreateAuditEntries(DbContext context)
    {

        context.ChangeTracker.DetectChanges();

        var auditEntries = new List<AuditEntry>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog)
                continue;

            if (entry.Entity is not IAuditable)
                continue;

            if (entry.State is EntityState.Detached or EntityState.Unchanged)
                continue;

            var auditEntry = CreateAuditEntry(entry);

            if (auditEntry is not null)
            {
                auditEntries.Add(auditEntry);
            }
        }

        return auditEntries;
    }

  

    private static string? GetEntityId(EntityEntry entry)
    {
        if (entry.Entity is BaseEntity entity)
        {
            return entity.Id.ToString();
        }

        return null;
    }
    private static string GetModifiedAction(EntityEntry entry)
    {
        var isDeletedProperty = entry.Properties
            .FirstOrDefault(x => x.Metadata.Name == "IsDeleted");

        if (isDeletedProperty is not null &&
            isDeletedProperty.OriginalValue is bool original &&
            isDeletedProperty.CurrentValue is bool current)
        {
            if (!original && current)
            {
                return AuditEntityActions.Deleted;
            }

            if (original && !current)
            {
                return AuditEntityActions.Restored;
            }
        }

        return AuditEntityActions.Updated;
    }
    private AuditEntry? CreateAuditEntry(EntityEntry entry)
    {

        
        var auditEntry = new AuditEntry(entry)
        {
            EntityType = entry.Entity.GetType().Name,
            EntityId = GetEntityId(entry)
        };

        switch (entry.State)
        {
            case EntityState.Added:
                auditEntry.Action = AuditEntityActions.Created;
                break;

            case EntityState.Modified:
                auditEntry.Action = GetModifiedAction(entry);
                break;

            default:
                return null;
        }

        CapturePropertyChanges(entry, auditEntry);

        if (auditEntry.Action == AuditEntityActions.Updated &&
            auditEntry.Changes.Count == 0)
        {
            return null;
        }

        return auditEntry;
    }

    private static readonly HashSet<string> IgnoredProperties =
 [
     nameof(BaseEntity.Id),
    nameof(BaseEntity.CreatedAtUtc),
    nameof(BaseEntity.UpdatedAtUtc),
    nameof(BaseEntity.RowVersion),
    "IsDeleted"
 ];

    private static void CapturePropertyChanges(
    EntityEntry entry,
    AuditEntry auditEntry)
    {
        foreach (var property in entry.Properties)
        {
            if (IgnoredProperties.Contains(property.Metadata.Name))
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.NewValues[property.Metadata.Name]
                        = property.CurrentValue;
                    break;

                case EntityState.Modified:

                    if (!property.IsModified)
                        continue;

                    if (Equals(
                            property.OriginalValue,
                            property.CurrentValue))
                    {
                        continue;
                    }

                    auditEntry.Changes.Add(
                        new PropertyChange
                        {
                            PropertyName = property.Metadata.Name,
                            OldValue = property.OriginalValue?.ToString(),
                            NewValue = property.CurrentValue?.ToString()
                        });

                    break;
            }
        }
    }

    private List<AuditLog> CreateAuditLogs(
    List<AuditEntry> auditEntries)
    {
        var auditLogs = new List<AuditLog>();

        foreach (var auditEntry in auditEntries)
        {
            var metadata = new AuditMetadata
            {
                Action =
        auditEntry.Action == AuditEntityActions.Deleted ||
        auditEntry.Action == AuditEntityActions.Restored
        ? auditEntry.Action
        : null,

                NewValues = auditEntry.NewValues.Count > 0
        ? auditEntry.NewValues
        : null,

                Changes = auditEntry.Changes.Count > 0
        ? auditEntry.Changes
        : null
            };

            string? metadataJson = null;

            if (metadata.Action is not null ||
                metadata.NewValues is not null ||
                metadata.Changes is not null)
            {
                metadataJson = JsonSerializer.Serialize(metadata);
            }

            var auditLog = AuditLog.Create(
                action: auditEntry.Action,
                userId: _currentUser.UserId,
                tenantId: _currentUser.TenantId,
                entityType: auditEntry.EntityType,
                entityId: auditEntry.EntityId,
                metadataJson: metadataJson);

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }

}
