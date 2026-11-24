using System.Diagnostics;
using DonationHub.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Common.Results
{
    public static class ResultProblemDetailsMapper
    {
        public static IActionResult ToProblemDetails(this Result result,
            HttpContext httpContext, IDictionary<string, string[]>? validationErrors = null)
        {
            var error = result.Error;
            var statusCode = MapStatusCode(error.Type);
            var id = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            var detailes = new ProblemDetails
            {
                Status = statusCode,
                Type = "https://httpstatuses.io/{statusCode}",
                Title = error.Description,
                Instance = httpContext.Request.Path
            };

            detailes.Extensions["code"] = error.Code;
            detailes.Extensions["traceId"] = id;

            if (error.Code == "Validation.Failed" &&
                validationErrors is { Count: > 0 })
            {
                detailes.Extensions["errors"] = validationErrors;
            }

            return new ObjectResult(detailes)
            {
                StatusCode = statusCode,
                ContentTypes = { "application/problem+json" }
            };
        }
        private static int MapStatusCode(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.PayloadTooLarge => StatusCodes.Status413PayloadTooLarge,
                ErrorType.UnprocessableEntity => StatusCodes.Status422UnprocessableEntity,
                ErrorType.TooManyRequests => StatusCodes.Status429TooManyRequests,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}

