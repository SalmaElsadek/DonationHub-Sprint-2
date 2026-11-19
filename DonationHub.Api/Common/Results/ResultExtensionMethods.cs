using DonationHub.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Common.Results
{
    public static class ResultExtensionMethods
    {
        public static IActionResult ToActionResult(this Result result,int successCode = StatusCodes.Status200OK)
        {
            if (result.IsSuccess) return new StatusCodeResult(successCode);

            return MapErrors(result.Error);
        }

        public static IActionResult ToActionResult<TValue>(this Result<TValue> result,
            Func<TValue,IActionResult>? onSuccess = null)
        {
            if (result.IsSuccess) return onSuccess is not null ? onSuccess(result.Value) :
                    new OkObjectResult(result.Value);

            return MapErrors(result.Error);
        }

        private static IActionResult MapErrors(Error error)
        {
            var statusCode = error.Type switch
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

            var problemDetails = new ProblemDetails
            {
                Status= statusCode,
                Title=error.Code,
                Detail=error.Description
            };

            problemDetails.Extensions["code"] = error.Code;

            return new ObjectResult(problemDetails)
            {
                StatusCode = statusCode,
            };
        }
    }
}
