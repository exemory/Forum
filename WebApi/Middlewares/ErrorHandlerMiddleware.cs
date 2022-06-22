using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Service.Exceptions;

namespace WebApi.Middlewares
{
    /// <summary>
    /// Middleware for handling application exceptions and converting them to responses
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor for initializing a <see cref="ErrorHandlerMiddleware"/> class instance
        /// </summary>
        /// <param name="next">Next middleware in request pipeline</param>
        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Middleware logic for handling exceptions and convert them to the corresponding responses
        /// </summary>
        /// <param name="context">Context of current http request</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
            }
            catch (AuthenticationException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            catch (ForumException e)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(e.Message);
            }
        }
    }
}