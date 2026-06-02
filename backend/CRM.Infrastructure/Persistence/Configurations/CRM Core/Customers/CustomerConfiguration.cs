using CRM.Domain.CRM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration
    : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        //----------------------------------------
        // Primary Key
        //----------------------------------------

        builder.HasKey(x => x.Id);

        //----------------------------------------
        // Properties
        //----------------------------------------

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(Customer.MaxNameLength)
            .IsRequired();

        builder.Property(x => x.Industry)
            .HasMaxLength(100);

        builder.Property(x => x.Website)
            .HasMaxLength(300);

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


        //----------------------------------------
        // Indexes
        //----------------------------------------

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.IsDeleted,
            x.Name
        })
        .HasDatabaseName("IX_Customers_Tenant_Name");

        builder.HasIndex(x => new
        {
            x.TenantId,
            x.Status
        })
        .HasDatabaseName("IX_Customers_Tenant_Status");

        //----------------------------------------
        // Soft Delete Filter
        //----------------------------------------

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}