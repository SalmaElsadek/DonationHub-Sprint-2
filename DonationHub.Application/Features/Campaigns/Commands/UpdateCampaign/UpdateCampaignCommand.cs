using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Commands.UpdateCampaign
{
    public record UpdateCampaignCommand(
        Guid Id,
        string TitleAr,
        string TitleEn,
        string DescriptionAr,
        string DescriptionEn,
        decimal GoalAmount,
        string GoalCurrency,
        DateTimeOffset EndDate,
        Guid CategoryId,
        byte[] RowVersion) : IRequest<Result>;

}
