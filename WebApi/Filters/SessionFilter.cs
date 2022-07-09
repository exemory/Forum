using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;

namespace WebApi.Filters
{
    /// <summary>
    /// Action filter for session initialization
    /// </summary>
    public class SessionFilter : IAsyncActionFilter
    {
        /// <summary>
        /// Session filter logic
        /// </summary>
        /// <param name="context">Context of the current http request</param>
        /// <param name="next">Next action filter in pipeline</param>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var session = serviceProvider.GetRequiredService<ISession>();

            var claims = context.HttpContext.User;

            if (claims.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                var userId = new Guid(claims.FindFirstValue(ClaimTypes.NameIdentifier));
                var userRoles = claims.FindAll(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                session.Initialize(userId, userRoles);
            }

            await next();
        }
    }
}