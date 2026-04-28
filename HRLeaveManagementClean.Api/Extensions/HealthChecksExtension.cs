using Hangfire;
using HealthChecks.UI.Client;
using HRLeaveManagementClean.Api.Filters;
using HRLeaveManagementClean.Api.HealthChecks;
using HRLeaveManagementClean.Api.Jobs;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HRLeaveManagementClean.Api.Extensions
{
    public static class HealthChecksExtension
    {
        public static IServiceCollection HealthChecks(this IServiceCollection services,WebApplicationBuilder builder) 
        {
            services.AddHealthChecks()
            .AddSqlServer(
                connectionString: builder.Configuration
                                .GetConnectionString("HrDatabaseConnectionString")!,
                name: "sql-server",
                tags: new[] { "db", "sql" })

            .AddCheck<DatabaseHealthCheck>(
                name: "identity-database",
                tags: new[] { "db", "identity" })

            .AddCheck<MemoryHealthCheck>(
                name: "memory",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "system" })

            .AddHangfire(
                setup: o => { o.MinimumAvailableServers = 1; },
                name: "hangfire",
                tags: new[] { "jobs" });

                    // HealthChecks UI
                    services.AddHealthChecksUI(options =>
                    {
                        options.SetEvaluationTimeInSeconds(30);  
                        options.MaximumHistoryEntriesPerEndpoint(50);
                        options.AddHealthCheckEndpoint("HR System", "/health");
                    })
            .AddInMemoryStorage();

            return services;
        }

        public static IApplicationBuilder UseHealthChecksMiddleware(this WebApplication app)
        {
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false  
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("db"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.MapHealthChecksUI(options =>
            {
                options.UIPath = "/healthchecks-ui";
                options.ApiPath = "/healthchecks-api";
            });

            return app;
        }
    }
}
