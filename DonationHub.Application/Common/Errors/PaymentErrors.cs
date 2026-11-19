using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;

namespace DonationHub.Application.Common.Errors
{
    public static class PaymentErrors
    {
        public static readonly Error InvalidSignature = Error.Validation(
            "Payment.InvalidSignature",
            "The payment signature is invalid.");
    }
}
