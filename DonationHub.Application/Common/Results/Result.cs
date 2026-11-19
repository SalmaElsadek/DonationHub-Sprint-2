using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Application.Common.Results
{
    public class Result
    {
        protected Result(bool isSuccess,Error error)
        {
            if (isSuccess && error != Error.None)
            {
                throw new InvalidOperationException("Successful result cannot contain an error.");
            }

            if (!isSuccess && error == Error.None){
                throw new InvalidOperationException("Failed result must contain an error.");
}
        }
    }
}
