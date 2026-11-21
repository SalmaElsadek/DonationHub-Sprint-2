using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Features.Campaigns.Helpers;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Enum;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Queries.GetActiveCampaigns
{
    internal class GetActiveCampaignsQueryHandler : IRequestHandler<GetActiveCampaignsQuery, Result<IReadOnlyList<PublicCampaignDto>>>
    {
        private readonly TimeProvider _timeProvider;
        private readonly ICampaignReadRepository _campaignRead;

        public GetActiveCampaignsQueryHandler(TimeProvider timeProvider,ICampaignReadRepository campaignRead)
        {
            _timeProvider = timeProvider;
            _campaignRead = campaignRead;
        }
        public async Task<Result<IReadOnlyList<PublicCampaignDto>>> Handle(GetActiveCampaignsQuery request, CancellationToken cancellationToken)
        {
            var campaigns = await _campaignRead.GetByStatusAsync(CampaignStatus.Active, cancellationToken);
            var time=_timeProvider.GetUtcNow();

            var result = campaigns.Select(c =>
            {
                var title = CampaignQueryHelpers.PreferdLanguage(request.AcceptLanguage, c.TitleAr, c.TitleEn);
                var description = CampaignQueryHelpers.PreferdLanguage(request.AcceptLanguage, c.DescriptionAr, c.DescriptionEn);

                return new PublicCampaignDto
                {
                    Id = c.Id,
                    Title = title,
                    Description = description,
                    GoalAmount = c.GoalAmount.Amount,
                    GoalCurrency = c.GoalAmount.Currency,
                    RaisedAmount = c.RaisedAmount,
                    EndDate = c.EndDate,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category?.NameEn ?? string.Empty,
                    BannerFileName = c.BannerFileName,
                    Status = c.Status.ToString(),
                    ProgressPercent = CampaignQueryHelpers.CalculateProgressPercent(
                        c.RaisedAmount,
                        c.GoalAmount.Amount),
                    DaysLeft = (c.EndDate - time).Days
                };
            }).ToList();

            return Result.Success<IReadOnlyList<PublicCampaignDto>>(result);
        }
    }
}
