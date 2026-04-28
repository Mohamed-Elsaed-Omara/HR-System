using HRLeaveManagement.Identity.DbContext;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HRLeaveManagementClean.Api.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly HrLeaveManagementIdentityDbContext _context;

        public DatabaseHealthCheck(HrLeaveManagementIdentityDbContext context)
            => _context = context;
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

                return canConnect
                    ? HealthCheckResult.Healthy("Database is reachable.")
                    : HealthCheckResult.Unhealthy("Cannot connect to database.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    description: "Database check failed.",
                    exception: ex);
            }
        }
    }
}
