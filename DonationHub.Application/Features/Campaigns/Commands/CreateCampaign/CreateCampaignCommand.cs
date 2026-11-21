using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Features.Campaigns.DTOs;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Commands.CreateCampaign
{
    public record CreateCampaignCommand(string TitleAr,
        string TitleEn,
        string DescriptionAr,
        string DescriptionEn,
        decimal GoalAmount,
        string GoalCurrency,
        DateTimeOffset EndDate,
        Guid CategoryId,
        Guid CreatedByUserId,
        string? BannerFileName=null) : IRequest<Result<AdminCampaignDto>>;

}
