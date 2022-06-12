using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Service.Exceptions;

namespace WebApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

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