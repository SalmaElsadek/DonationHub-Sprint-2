using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enum;

namespace DonationHub.Application.Interfaces
{
    public interface ICampaignReadRepository
    {
        Task<IReadOnlyList<Campaign>> GetByStatusAsync(CampaignStatus status, CancellationToken ct);
        Task<Campaign?>GetByIdWithCategoryAsync(Guid id,CancellationToken ct);
    }
}
