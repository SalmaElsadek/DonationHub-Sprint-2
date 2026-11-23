using System;
using System.Reflection;
using DonationHub.Application.Common.Results;
using FluentValidation;
using MediatR;

namespace DonationHub.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = results
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count == 0)
                return await next(cancellationToken);

            var errors = failures.GroupBy(f => f.PropertyName)
                .ToDictionary(
                g => g.Key,
                g => g.Select(f=>f.ErrorMessage).Distinct().ToArray()
                );

            return CreateValidationFailureResponse(errors);
        }

        private static TResponse CreateValidationFailureResponse(
            Dictionary<string, string[]> errors)
        {
            //بدل ما اعمل اوفر لود لل failure
            var details = string.Join(" | ", errors.SelectMany(e => e.Value));
            var error = new Error(
                "Validation.Failed",
                string.IsNullOrWhiteSpace(details)
                    ? "One or more validation errors occurred."
                    : details,
                ErrorType.Validation);

            if (typeof(TResponse) == typeof(Result))
            {
                var result = Result.Failure(error);
                return (TResponse)(object)result;
            }

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var type=typeof(TResponse).GetGenericArguments()[0];

                var failure = typeof(Result<>)
                    .MakeGenericType(type)
                    .GetMethod(
                        nameof(Result<object>.Failure),
                        BindingFlags.Public | BindingFlags.Static,
                        binder: null,
                        types: [typeof(Error), typeof(Dictionary<string, string[]>)],
                        modifiers: null);

                if (failure is not null)
                {
                    var result = failure.Invoke(null, [error, errors]);
                    if (result is TResponse typedResult)
                        return typedResult;
                }
            }
            throw new InvalidOperationException(
                $"Cannot build validation failure response for type '{typeof(TResponse).Name}'");
        }
    }
}