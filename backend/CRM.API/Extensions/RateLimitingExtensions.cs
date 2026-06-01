using System.Threading.RateLimiting;
using CRM.API.Responses;
using CRM.Shared.Constants;
using Microsoft.AspNetCore.RateLimiting;

namespace CRM.API.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimitingPolicies(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Global response
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString();
                }

                var response = new ErrorResponse
                {
                    ErrorCode = "RATE_LIMIT_EXCEEDED",
                    Message = "Too many requests. Please try again later.",
                    TraceId = context.HttpContext.TraceIdentifier
                };

                await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            };

            // LOGIN POLICY
            options.AddPolicy(
                RateLimitPolicies.LoginPolicy,
                httpContext =>
                {
                    var ipAddress =
                        httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                });

            // REGISTER POLICY
            options.AddPolicy(
                RateLimitPolicies.RegisterPolicy,
                httpContext =>
                {
                    var ipAddress =
                        httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAddress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromHours(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                });

            // REFRESH TOKEN POLICY
            options.AddPolicy(
                RateLimitPolicies.RefreshPolicy,
                httpContext =>
                {
                    var partitionKey =
                        httpContext.User?.Identity?.Name
                        ?? httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "anonymous";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: partitionKey,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                });
        });

        return services;
    }
}
