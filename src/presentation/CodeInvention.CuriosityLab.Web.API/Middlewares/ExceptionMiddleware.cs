using CodeInvention.CuriosityLab.Application.Common.Exceptions;
using System.Diagnostics;
using System.Net;

namespace CodeInvention.CuriosityLab.Web.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleException(context, exception);
            }
        }

        private async Task HandleException(HttpContext context, Exception exception)
        {
            if (exception is ValidationException validationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { validationException.Message, validationException.Failures });
                return;
            }
            //if (exception is NotFoundException notFoundException)
            //{
            //    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            //    context.Response.ContentType = "application/json";
            //    await context.Response.WriteAsJsonAsync(new { notFoundException.Message });
            //    return;
            //}
            //if (exception is BadRequestException badRequestException)
            //{
            //    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    context.Response.ContentType = "application/json";
            //    await context.Response.WriteAsJsonAsync(new { badRequestException.Code, badRequestException.Message });

            //    return;
            //}
            //if (exception is UnauthorizedException unauthorizedException)
            //{
            //    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //    context.Response.ContentType = "application/json";
            //    await context.Response.WriteAsJsonAsync($"{unauthorizedException.Message}");
            //    return;
            //}
            //if (exception is ForbiddenException forbiddenException)
            //{
            //    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            //    context.Response.ContentType = "application/json";
            //    await context.Response.WriteAsJsonAsync($"{forbiddenException.Message}");
            //    return;
            //}
            else
            {
                //Using the request's trace id so the id returned to the client matches the TraceId on every log entry of the request
                var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

                //Logging before writing the response so the error is kept even if writing the response fails
                _logger.LogError(exception, "Unhandled exception on {Method} {Path} (TraceId: {TraceId})", context.Request.Method, context.Request.Path, traceId);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { exception.Message, TraceId = traceId });
            }
        }
    }
}
