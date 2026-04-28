using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace HRLeaveManagementClean.Api.Extensions
{
    public static class RateLimitExtension
    {
        public static IServiceCollection AddRateLimitExtension(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Policy 1:  Auth endpoints (login, register, forgot-password)

                options.AddFixedWindowLimiter("AuthPolicy", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(15);
                    opt.PermitLimit = 10;
                    opt.QueueLimit = 0;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                // Policy 2:  API General (endpoints)

                options.AddFixedWindowLimiter("GeneralPolicy", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.PermitLimit = 100;
                    opt.QueueLimit = 0;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                // Policy 3:  IP
                options.AddPolicy("PerIpPolicy", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(ip, _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 30,
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        });
                });

                // limit  Retry-After header
                options.OnRejected = async (context, cancellationToken) =>
                {
                    var response = context.HttpContext.Response;
                    response.StatusCode = StatusCodes.Status429TooManyRequests;
                    response.ContentType = "application/json";

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        response.Headers["Retry-After"] = ((int)retryAfter.TotalSeconds).ToString();
                        response.Headers["X-RateLimit-Reset"] =
                            DateTimeOffset.UtcNow.Add(retryAfter).ToUnixTimeSeconds().ToString();
                    }

                    await response.WriteAsync(
                        System.Text.Json.JsonSerializer.Serialize(new
                        {
                            statusCode = 429,
                            message = "Too many requests. Please try again later.",
                            retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var r)
                                         ? (int)r.TotalSeconds : 0
                        }), cancellationToken);
                };
            });



            return services;
        }
    }
}
