using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Interfaces.Persistence;
using CRM.Application.CRM.Customers.Interfaces;
using CRM.Application.Identity.Interfaces;
using CRM.Infrastructure.Identity;
using CRM.Infrastructure.Identity.Google;
using CRM.Infrastructure.Persistence;
using CRM.Infrastructure.Persistence.Repositories.Identity;
using CRM.Infrastructure.Repositories;
using CRM.Infrastructure.Repositories.Identity;
using CRM.Infrastructure.Services;
using CRM.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace CRM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            //for email verification
            services.AddScoped<ITokenGenerator, TokenGenerator>();


            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IExternalLoginRepository, ExternalLoginRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();

            //Email Verification
            services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

            //Email service
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));


            services.AddScoped<IEmailService, SmtpEmailService>();

            //OAuth Google
            services.Configure<GoogleAuthOptions>(configuration.GetSection("GoogleAuth"));

            services.AddScoped<IGoogleAuthService,GoogleAuthService>();




            //CRM Core Module....................................

            //Customer Repository
            services.AddScoped<ICustomerRepository, CustomerRepository>();


            // Generic repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            //Unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //claims fetch
            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUser, CurrentUser>();

            //AuditLog
            services.AddScoped<IAuditService, AuditService>();


            return services;
        }
    }
}
