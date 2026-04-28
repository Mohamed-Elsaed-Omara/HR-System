using Hangfire;
using HRLeaveManagement.Identity.DbContext;
using Microsoft.EntityFrameworkCore;

namespace HRLeaveManagementClean.Api.Jobs
{
    public class RefreshTokenCleanupJob
    {
        private readonly HrLeaveManagementIdentityDbContext _context;
        private readonly ILogger<RefreshTokenCleanupJob> _logger;

        public RefreshTokenCleanupJob(
            HrLeaveManagementIdentityDbContext context,
            ILogger<RefreshTokenCleanupJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 600 })]
        public async Task Execute()
        {
            _logger.LogInformation("Starting refresh token cleanup at {Time}", DateTime.UtcNow);

            var cutoff = DateTime.UtcNow;
            var expired = _context.UserRefreshTokens
                .Where(t => t.ExpiresAt < cutoff || t.IsRevoked);

            var count = await expired.CountAsync();
            _context.UserRefreshTokens.RemoveRange(expired);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleanup done. Removed {Count} expired tokens", count);
        }
    }

}
