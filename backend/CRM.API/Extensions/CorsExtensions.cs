namespace CRM.API.Extensions;

public static class CorsExtensions
{
    public const string SpaPolicy = "SpaCors";

    public static IServiceCollection AddSpaCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:5173", "https://localhost:5173"];

        services.AddCors(options =>
        {
            options.AddPolicy(SpaPolicy, policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
