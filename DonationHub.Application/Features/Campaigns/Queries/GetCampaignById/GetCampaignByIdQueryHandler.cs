using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Features.Campaigns.Helpers;
using DonationHub.Application.Interfaces;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Queries.GetCampaignById
{
    public class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQuery, Result<CampaignDetailsDto>>
    {
        private readonly TimeProvider _timeProvider;
        private readonly ICampaignReadRepository _campaignRead;
        public GetCampaignByIdQueryHandler(TimeProvider timeProvider, ICampaignReadRepository campaignRead)
        {
            _timeProvider = timeProvider;
            _campaignRead = campaignRead;
        }
        public async Task<Result<CampaignDetailsDto>> Handle(GetCampaignByIdQuery request, CancellationToken cancellationToken)
        {
            var time=_timeProvider.GetUtcNow(); 
            var campaign=await _campaignRead.GetByIdWithCategoryAsync(request.Id, cancellationToken);

            if (campaign == null)
                return Result.Failure<CampaignDetailsDto>(CampaignErrors.NotFound);

            var title = CampaignQueryHelpers.PreferdLanguage(request.AcceptLanguage, campaign.TitleAr, campaign.TitleEn);
            var description = CampaignQueryHelpers.PreferdLanguage(request.AcceptLanguage, campaign.DescriptionAr, campaign.DescriptionEn);
            var categoryName=campaign.Category is null? string.Empty:
                CampaignQueryHelpers.PreferdLanguage(request.AcceptLanguage,campaign.Category.NameAr, campaign.Category.NameEn);

            var result = new CampaignDetailsDto {
                Id = campaign.Id,
                Title = title,
                Description = description,
                GoalAmount = campaign.GoalAmount.Amount,
                GoalCurrency = campaign.GoalAmount.Currency,
                RaisedAmount = campaign.RaisedAmount,
                EndDate = campaign.EndDate,
                CategoryId = campaign.CategoryId,
                CategoryName = categoryName,
                BannerFileName = campaign.BannerFileName,
                Status = campaign.Status.ToString(),
                ProgressPercent = CampaignQueryHelpers.CalculateProgressPercent(
                    campaign.RaisedAmount,
                    campaign.GoalAmount.Amount),
                DaysLeft = (campaign.EndDate-time).Days
            };

            if (request.IncludeBilingualDetails)
            {
                result.TitleAr = campaign.TitleAr;
                result.TitleEn = campaign.TitleEn;
                result.DescriptionAr = campaign.DescriptionAr;
                result.DescriptionEn = campaign.DescriptionEn;
                result.CreatedByUserId = campaign.CreatedByUserId;
                result.UpdatedAt = campaign.UpdatedAt;
                result.RowVersion = campaign.RowVersion;
            }

            return Result.Success(result);

        }
    }
}
