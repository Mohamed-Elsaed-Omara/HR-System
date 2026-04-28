using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace HRLeaveManagementClean.Api.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return httpContext.User.Identity?.IsAuthenticated == true
                && httpContext.User.IsInRole("ADMINISTRATOR");
            //return true;
        }
    }
}
