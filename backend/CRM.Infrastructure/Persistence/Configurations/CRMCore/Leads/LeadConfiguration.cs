using CRM.Domain.CRM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Persistence.Configurations;

public sealed class LeadConfiguration
    : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("Leads");

        //----------------------------------------
        // Primary Key
        //----------------------------------------

        builder.HasKey(x => x.Id);

        //----------------------------------------
        // Properties
        //----------------------------------------

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(Lead.MaxFirstNameLength);

        builder.Property(x => x.LastName)
            .HasMaxLength(Lead.MaxLastNameLength);

        builder.Property(x => x.Email)
            .HasMaxLength(Lead.MaxEmailLength);

        builder.Property(x => x.Phone)
            .HasMaxLength(Lead.MaxPhoneLength);

        builder.Property(x => x.Company)
            .HasMaxLength(Lead.MaxCompanyLength);

        builder.Property(x => x.Score)
            .HasPrecision(5, 2);

        builder.Property(x => x.ScoreVersion)
            .HasMaxLength(Lead.MaxScoreVersionLength);

        builder.Property(x => x.OwnerUserId);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        //----------------------------------------
        // Enum Conversion
        //----------------------------------------

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Source)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();



        //----------------------------------------
        // Indexes
        //----------------------------------------

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.IsDeleted,
            x.Status
        })
        .HasDatabaseName("IX_Leads_Tenant_Status");

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.Email
        })
        .HasDatabaseName("IX_Leads_Tenant_Email");

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.Source
        })
        .HasDatabaseName("IX_Leads_Tenant_Source");

        

        //----------------------------------------
        // Soft Delete Filter
        //----------------------------------------

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}