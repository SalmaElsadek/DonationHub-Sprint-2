using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Errors;

namespace DonationHub.Application.Common.Results
{
    public class Result
    {
        protected Result(bool isSuccess,Error error, Dictionary<string, string[]>?validationErrors=null) 
        {

            if (isSuccess && error != Error.None)
            {
                throw new InvalidOperationException("Successful result cannot contain an error.");
            }

            if (!isSuccess && error == Error.None){
                throw new InvalidOperationException("Failed result must contain an error.");
            }

            IsSuccess= isSuccess;
            Error= error;
            ValidationErrors = validationErrors;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public Dictionary<string, string[]>? ValidationErrors {  get; }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
        public static Result Failure(Error error, Dictionary<string, string[]> validationErrors)
            =>new(false, error, validationErrors);

        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
        public static Result<TValue> Failure<TValue>(Error error, Dictionary<string, string[]> validationErrors) 
            =>new(default,false, error);
    }

    public sealed class Result<TValue> : Result
    {
        private TValue? _value;

        internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
        {
            _value = value;
        }
        public TValue Value=>IsSuccess?_value!
            : throw new InvalidOperationException("Cannot access Value of a failed result.");

        public static implicit operator Result<TValue>(TValue? value) =>
            value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

        public static implicit operator Result<TValue>(Error error) =>
            Failure<TValue>(error);
    }
}
