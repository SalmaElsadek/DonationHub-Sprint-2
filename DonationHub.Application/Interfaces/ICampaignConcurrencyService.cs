using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Domain.Entities;

namespace DonationHub.Application.Interfaces
{
    public interface ICampaignConcurrencyService
    {
        void SetOriginalRowVersion(Campaign campaign, byte[] rowVersion);
    }
}
