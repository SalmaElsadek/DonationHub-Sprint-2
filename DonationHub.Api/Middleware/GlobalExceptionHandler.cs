using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler>logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var id=Activity.Current?.Id ?? httpContext.TraceIdentifier;

            _logger.LogError(exception, "Unhandled exception occurred with TraceId: {id}, Path: {Path}",
                id,
                httpContext.Request.Path);

            var detailes = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://httpstatuses.io/500",
                Title = "Unexpected error occurred while doing the request",
                Detail = "An internal server error occurred",
                Instance = httpContext.Request.Path
            };

            detailes.Extensions["code"] = "Server.UnhandledError";
            detailes.Extensions["traceId"] = id;

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(detailes,cancellationToken);
            return true;
        }
    }
}
