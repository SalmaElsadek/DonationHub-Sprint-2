using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;

namespace DonationHub.Application.Common.Errors
{
    public static class CommonErrors
    {
        public static readonly Error RateLimited = Error.TooManyRequests(
            "Common.RateLimited",
            "Rate limit exceeded. Please try again later.");
    }
}
