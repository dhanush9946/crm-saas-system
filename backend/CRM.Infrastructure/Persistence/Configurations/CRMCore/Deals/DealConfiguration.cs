using CRM.Domain.CRM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Persistence.Configurations;

public sealed class DealConfiguration
    : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.ToTable("Deals");

        //----------------------------------------
        // Primary Key
        //----------------------------------------

        builder.HasKey(x => x.Id);

        //----------------------------------------
        // Properties
        //----------------------------------------

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(Deal.MaxTitleLength)
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.LeadId);

        builder.Property(x => x.Value)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Probability)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.ExpectedCloseDate);

        builder.Property(x => x.OwnerUserId);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        //----------------------------------------
        // Enum Conversion
        //----------------------------------------

        builder.Property(x => x.Stage)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        //----------------------------------------
        // Indexes
        //----------------------------------------

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.IsDeleted,
            x.Stage
        })
        .HasDatabaseName("IX_Deals_Tenant_Stage");

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.CustomerId
        })
        .HasDatabaseName("IX_Deals_Tenant_Customer");

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.OwnerUserId
        })
        .HasDatabaseName("IX_Deals_Tenant_Owner");

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.ExpectedCloseDate
        })
        .HasDatabaseName("IX_Deals_Tenant_CloseDate");

        //----------------------------------------
        // Soft Delete Filter
        //----------------------------------------

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}