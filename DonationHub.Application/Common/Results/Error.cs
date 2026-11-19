using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Application.Common.Results
{
    //data/value object =>no complex behavior
    public sealed record Error(string Code,string Description,ErrorType Type)
    {
        //For Successful result that cannot contain an error
        public static readonly Error None = new Error(string.Empty, string.Empty, ErrorType.Failure);
        public static readonly Error NullValue = new("General.NullValue", "A null value.", ErrorType.Validation);

        public static Error Validation(string code, string description)=>
            new(code,description,ErrorType.Validation);
        public static Error NotFound(string code, string description) =>
            new(code, description, ErrorType.NotFound);

        public static Error Conflict(string code, string description) =>
            new(code, description, ErrorType.Conflict);

        public static Error Unauthorized(string code, string description) =>
            new(code, description, ErrorType.Unauthorized);

        public static Error Forbidden(string code, string description) =>
            new(code, description, ErrorType.Forbidden);
        public static Error UnprocessableEntity(string code, string description) =>
            new(code, description, ErrorType.UnprocessableEntity);

        public static Error PayloadTooLarge(string code, string description) =>
            new(code, description, ErrorType.PayloadTooLarge);

        public static Error TooManyRequests(string code, string description) =>
            new(code, description, ErrorType.TooManyRequests);

        public static Error Failure(string code, string description) =>
            new(code, description, ErrorType.Failure);

    }
}
