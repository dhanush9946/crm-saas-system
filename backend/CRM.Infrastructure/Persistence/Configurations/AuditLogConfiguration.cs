using CRM.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.EntityType)
            .HasMaxLength(100);

        builder.Property(x => x.EntityId)
            .HasMaxLength(100);

        builder.Property(x => x.Succeeded)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(45);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);

        builder.Property(x => x.DeviceId)
            .HasMaxLength(200);

        builder.Property(x => x.TraceId)
            .HasMaxLength(100);

        builder.Property(x => x.MetadataJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(x => new { x.TenantId, x.CreatedAtUtc });

        builder.HasIndex(x => new { x.UserId, x.CreatedAtUtc });

        builder.HasIndex(x => new { x.TenantId, x.Action, x.CreatedAtUtc });

        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.EntityType,
            x.EntityId,
            x.CreatedAtUtc
        })
        .HasDatabaseName("IX_AuditLogs_EntityHistory");
    }
}
