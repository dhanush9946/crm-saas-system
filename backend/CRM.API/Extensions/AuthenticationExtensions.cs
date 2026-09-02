using CRM.API.Responses;
using CRM.Application.Identity.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CRM.API.Extensions
{
    public static class AuthenticationExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web);

        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            var key = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
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
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();

                            context.Response.StatusCode =
                                (int)HttpStatusCode.Unauthorized;

                            context.Response.ContentType =
                                "application/json";

                            context.Response.Headers.WWWAuthenticate =
                                "Bearer";

                            var hasAuthorizationHeader =
                                context.Request.Headers
                                    .ContainsKey("Authorization");

                            var response = new ErrorResponse
                            {
                                ErrorCode = "UNAUTHORIZED",

                                Message = hasAuthorizationHeader
                                    ? "Authentication token is invalid or expired"
                                    : "Authentication is required",

                                TraceId =
                                    context.HttpContext.TraceIdentifier
                            };

                            var json = JsonSerializer.Serialize(
                                response,
                                JsonOptions);

                            await context.Response.WriteAsync(json);
                        },

                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode =
                                (int)HttpStatusCode.Forbidden;

                            context.Response.ContentType =
                                "application/json";

                            var response = new ErrorResponse
                            {
                                ErrorCode = "FORBIDDEN",

                                Message =
                                    "You do not have permission to access this resource",

                                TraceId =
                                    context.HttpContext.TraceIdentifier
                            };

                            var json = JsonSerializer.Serialize(
                                response,
                                JsonOptions);

                            await context.Response.WriteAsync(json);
                        }
                    };
                });

            return services;
        }
    }
}