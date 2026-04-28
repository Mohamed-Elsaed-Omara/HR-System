using Hangfire;
using Hangfire.SqlServer;
using HRLeaveManagementClean.Api.Filters;
using HRLeaveManagementClean.Api.Jobs;

namespace HRLeaveManagementClean.Api.Extensions
{
    public static class HangfireExtension
    {
        public static IServiceCollection AddHangfire(this IServiceCollection services,WebApplicationBuilder builder) 
        {
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
            builder.Configuration.GetConnectionString("HrDatabaseConnectionString"),

            new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 2; 
            });

            return services;
        }

        public static IApplicationBuilder UseHangfireMiddleware(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            // سجّل الـ recurring job
            RecurringJob.AddOrUpdate<RefreshTokenCleanupJob>(
                recurringJobId: "refresh-token-cleanup",
                methodCall: job => job.Execute(),
                cronExpression: Cron.Daily(hour: 3)  
            );

            return app;
        }
    }
}
