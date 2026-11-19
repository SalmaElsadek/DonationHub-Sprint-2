using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;

namespace DonationHub.Application.Common.Errors
{
    public static class CampaignErrors
    {
        public static readonly Error NotFound = Error.NotFound(
            "Campaign.NotFound",
            "The campaign was not found.");

        public static readonly Error NotActive = Error.Conflict(
            "Campaign.NotActive",
            "The campaign is completed or expired and cannot accept this action.");

        public static readonly Error GoalBelowRaised = Error.UnprocessableEntity(
            "Campaign.GoalBelowRaised",
            "Target goal cannot be lower than the amount already raised.");

        public static readonly Error EndDateInPast = Error.UnprocessableEntity(
            "Campaign.EndDateInPast",
            "Campaign end date must be in the future.");

        public static readonly Error ConcurrencyConflict = Error.Conflict(
            "Campaign.ConcurrencyConflict",
            "A concurrency conflict occurred while updating the campaign.");
    }
}
