using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;

namespace DonationHub.Application.Common.Errors
{
    public static class FileErrors
    {
        public static readonly Error InvalidType = Error.Validation(
            "File.InvalidType",
            "The file type or signature is not allowed.");

        public static readonly Error TooLarge = Error.PayloadTooLarge(
            "File.TooLarge",
            "The uploaded file exceeds the maximum allowed size.");
    }
}
