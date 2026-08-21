using CRM.Domain.Common;
using CRM.Domain.CRM.Entities;
using CRM.Domain.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CRM.Infrastructure.Persistence
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //Identity

        public DbSet<User> Users => Set<User>();

        public DbSet<Tenant> Tenants => Set<Tenant>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
        public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();


        //CRM Core 

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Lead> Leads => Set<Lead>();
        public DbSet<Deal> Deals => Set<Deal>();
        public DbSet<Activity> Activities => Set<Activity>();

        public DbSet<LeadConversionHistory> LeadConversionHistories
                                     => Set<LeadConversionHistory>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entity.ClrType))
                {
                    var property = modelBuilder.Entity(entity.ClrType)
                        .Property(nameof(BaseEntity.RowVersion))
                        .IsRequired()
                        .IsRowVersion()
                        .ValueGeneratedOnAddOrUpdate();

                    property.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
                    property.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                }
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
