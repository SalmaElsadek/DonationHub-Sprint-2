using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enum;
using DonationHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Repositories
{
    public class CampaignReadRepository : ICampaignReadRepository
    {
        private readonly AppDbContext _context;

        public CampaignReadRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Campaign?> GetByIdWithCategoryAsync(Guid id, CancellationToken ct)
        {
            return await _context.Campaigns.AsNoTracking()
                .Include(c=>c.Category).
                FirstOrDefaultAsync(c=>c.Id == id,ct);
        }

        public async Task<IReadOnlyList<Campaign>> GetByStatusAsync(CampaignStatus status, CancellationToken ct)
        {
            return await _context.Campaigns.AsNoTracking()
                .Include(c => c.Category).Where(c => c.Status == status)
                .OrderByDescending(c => c.Id).
                ToListAsync(ct);
        }
    }
}
