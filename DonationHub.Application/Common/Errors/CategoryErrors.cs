using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;

namespace DonationHub.Application.Common.Errors
{
    public static class CategoryErrors
    {
        public static readonly Error NotFound = Error.NotFound(
            "Category.NotFound",
            "The category was not found.");

        public static readonly Error HasCampaigns = Error.Conflict(
            "Category.HasCampaigns",
            "Cannot delete category because it has associated campaigns.");
    }
}
