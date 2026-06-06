using CRM.Domain.CRM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Persistence.Configurations.CRMCore.Activities;

public sealed class ActivityConfiguration
    : IEntityTypeConfiguration<Activity>
{
    public void Configure(
        EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("Activities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Subject)
            .HasMaxLength(Activity.MaxSubjectLength)
            .IsRequired();

        builder.Property(x => x.Notes);

        builder.Property(x => x.OccurredAtUtc);

        builder.Property(x => x.DueAtUtc);

        builder.Property(x => x.CompletedAtUtc);

        builder.Property(x => x.RelatedEntityType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.RelatedEntityId)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        

        builder.HasIndex(x =>
            new
            {
                x.TenantId,
                x.RelatedEntityType,
                x.RelatedEntityId
            });

        builder.HasIndex(x =>
            new
            {
                x.TenantId,
                x.DueAtUtc
            });

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}