using CRM.API.Extensions;
using CRM.API.Logging;
using CRM.API.Middleware;
using CRM.Application;
using CRM.Infrastructure;
using CRM.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Database
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sql => sql.EnableRetryOnFailure()
        ));

    // Application & Infrastructure
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructure();

    // Authentication & Authorization
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddAuthorization();

    //Rate Limiting
    builder.Services.AddRateLimitingPolicies();

    // API + Swagger
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerWithAuth();

    //Serilog
    builder.AddSerilogLogging();

    var app = builder.Build();

    // Middleware pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseSecurityHeaders();
    app.UseCorrelationId();

    app.UseSerilogRequestLogging();

    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseAuthentication();
    app.UseRateLimiter();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly during startup");
}
finally
{
    Log.CloseAndFlush();
}