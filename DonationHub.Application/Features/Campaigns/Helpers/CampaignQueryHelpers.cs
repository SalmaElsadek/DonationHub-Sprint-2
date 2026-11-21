using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonationHub.Application.Features.Campaigns.Helpers
{
    public static class CampaignQueryHelpers
    {
        public static string PreferdLanguage(string? language,string arValue,string enValue)
        {
            var result=language?.Trim().ToLowerInvariant();

            if (result == "ar" || (result is not null && result.StartsWith("ar-")))
                return arValue;
            return enValue;
        }

        public static double CalculateProgressPercent(decimal raisedAmount,decimal goalAmount)
        {
            if (goalAmount <= 0) return 0;

            var percent=(double)(raisedAmount / goalAmount)*100.0;
            return Math.Min(100.0,percent); 
        }
    }
}
