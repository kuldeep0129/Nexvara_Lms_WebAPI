using Azure;
using Domain.common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Nexvara.WebAPI.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);
            var respone = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };
            respone.Extensions["traceId"] = httpContext.TraceIdentifier;
            httpContext.Response.StatusCode =StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync( respone,  cancellationToken);
            return true;
        }
    }
}
