using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;

namespace DonationHub.Application.Common.Errors
{
    public static class AuthenticationErrors
    {
        public static readonly Error InvalidCredentials = Error.Unauthorized(
            "Auth.InvalidCredentials",
            "Invalid email or password.");

        public static readonly Error EmailAlreadyExists = Error.Conflict(
            "Auth.EmailAlreadyExists",
            "this email already exists.");

        public static readonly Error RefreshTokenInvalid = Error.Unauthorized(
            "Auth.RefreshTokenInvalid",
            "The refresh token is invalid, expired, or has been revoked.");

        public static readonly Error AccountDeactivated = Error.Forbidden(
            "Auth.AccountDeactivated",
            "The user account is deactivated.");
    }
}
