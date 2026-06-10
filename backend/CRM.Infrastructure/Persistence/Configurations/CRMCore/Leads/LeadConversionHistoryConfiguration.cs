using CRM.Domain.CRM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Persistence.Configurations;

public sealed class LeadConversionHistoryConfiguration
    : IEntityTypeConfiguration<LeadConversionHistory>
{
    public void Configure(
        EntityTypeBuilder<LeadConversionHistory> builder)
    {
        builder.ToTable("LeadConversionHistories");

        //----------------------------------------
        // Primary Key
        //----------------------------------------

        builder.HasKey(x => x.Id);

        //----------------------------------------
        // Properties
        //----------------------------------------

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.LeadId)
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.ConvertedByUserId)
            .IsRequired();

        builder.Property(x => x.ConvertedAtUtc)
            .IsRequired();

        //----------------------------------------
        // Indexes
        //----------------------------------------

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.LeadId
        })
        .HasDatabaseName(
            "IX_LeadConversionHistories_Tenant_Lead");

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.CustomerId
        })
        .HasDatabaseName(
            "IX_LeadConversionHistories_Tenant_Customer");

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.ConvertedAtUtc
        })
        .HasDatabaseName(
            "IX_LeadConversionHistories_Tenant_ConvertedAt");
    }
}