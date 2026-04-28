using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HRLeaveManagementClean.Api.HealthChecks
{
    public class MemoryHealthCheck : IHealthCheck
    {
        private readonly long _thresholdMb;

        public MemoryHealthCheck(long thresholdMb = 500)
        =>_thresholdMb = thresholdMb;
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context
            , CancellationToken cancellationToken = default)
        {
            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var allocatedMb = allocated / 1024 / 1024;

            var data = new Dictionary<string, object>
                {
                    { "AllocatedMegabytes", allocatedMb },
                    { "ThresholdMegabytes", _thresholdMb },
                    { "Gen0Collections", GC.CollectionCount(0) },
                    { "Gen1Collections", GC.CollectionCount(1) },
                    { "Gen2Collections", GC.CollectionCount(2) },
                };

            return _thresholdMb > allocatedMb
                    ? Task.FromResult(HealthCheckResult.Healthy($"Memory usage: {allocatedMb} MB", data))
                    : Task.FromResult(HealthCheckResult.Degraded($"Memory usage high: {allocatedMb} MB", data: data));
        }
    }
}
