using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;

namespace DonationHub.Application.Common.Errors
{
    public static class IdempotencyErrors
    {
        public static readonly Error KeyReusedWithDifferentRequest = Error.UnprocessableEntity(
            "Idempotency.KeyReusedWithDifferentRequest",
            "The provided Idempotency-Key was already used with a different request payload.");
    }
}
