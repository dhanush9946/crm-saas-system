using CRM.Application.Common.Interfaces;
using CRM.Application.Identity.Interfaces;
using CRM.Infrastructure.Persistence;
using CRM.Infrastructure.Repositories.Identity;
using CRM.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;


namespace CRM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();


            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();

            // Generic repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            //Unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            return services;
        }
    }
}
