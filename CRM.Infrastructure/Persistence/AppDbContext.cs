using CRM.Domain.Common;
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

        public DbSet<User> Users => Set<User>();

        public DbSet<Tenant> Tenants => Set<Tenant>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entity.ClrType))
                {
                    var property = modelBuilder.Entity(entity.ClrType)
                        .Property(nameof(BaseEntity.RowVersion))
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
