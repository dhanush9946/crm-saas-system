using CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRM.API.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            var key = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = issuer,
                        ValidAudience = audience,

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(key!))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var userIdValue = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                                ?? context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                                ?? context.Principal?.FindFirstValue("sub");

                            var tenantIdValue = context.Principal?.FindFirstValue("tenantId");
                            var tokenVersionValue = context.Principal?.FindFirstValue("ver");

                            if (!Guid.TryParse(userIdValue, out var userId) ||
                                !Guid.TryParse(tenantIdValue, out var tenantId) ||
                                !int.TryParse(tokenVersionValue, out var tokenVersion))
                            {
                                context.Fail("Invalid token claims");
                                return;
                            }

                            var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                            var user = await dbContext.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Id == userId, context.HttpContext.RequestAborted);

                            if (user == null ||
                                user.TenantId != tenantId ||
                                user.IsDisabled() ||
                                user.TokenVersion != tokenVersion)
                            {
                                context.Fail("Token version is no longer valid");
                            }
                        }
                    };
                });

            return services;
        }
    }
}
