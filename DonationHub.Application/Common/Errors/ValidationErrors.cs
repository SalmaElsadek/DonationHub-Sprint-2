using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;

namespace DonationHub.Application.Common.Errors
{
    public static class ValidationErrors
    {
        public static readonly Error Failed = Error.Validation(
            "Validation.Failed",
            "validation errors occurred.");
    }
}
