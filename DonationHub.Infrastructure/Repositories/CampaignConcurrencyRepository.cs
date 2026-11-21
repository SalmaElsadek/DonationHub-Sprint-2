using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Data;

namespace DonationHub.Infrastructure.Repositories
{
    public class CampaignConcurrencyRepository : ICampaignConcurrencyService
    {
        private readonly AppDbContext _context;

        public CampaignConcurrencyRepository(AppDbContext context)
        {
            _context = context;
        }
        public void SetOriginalRowVersion(Campaign campaign, byte[] rowVersion)
        {
            if(rowVersion is { Length: > 0 })
            {
                _context.Entry(campaign).Property(c => c.RowVersion)
                    .OriginalValue = rowVersion;
            }
        }
    }
}
