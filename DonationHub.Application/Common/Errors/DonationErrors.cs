using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;

namespace DonationHub.Application.Common.Errors
{
    public static class DonationErrors
    {
        public static readonly Error NotFound = Error.NotFound(
            "Donation.NotFound",
            "The donation was not found.");

        public static readonly Error AmountOutOfRange = Error.UnprocessableEntity(
            "Donation.AmountOutOfRange",
            "The donation amount is out of allowed range.");

        public static readonly Error InvalidState = Error.Conflict(
            "Donation.InvalidState",
            "The donation is already in a final state and cannot be modified.");
    }
}
