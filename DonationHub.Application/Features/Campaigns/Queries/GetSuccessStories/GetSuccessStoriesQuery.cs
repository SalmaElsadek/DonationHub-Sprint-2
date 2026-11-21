using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Features.Campaigns.DTOs;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Queries.GetSuccessStories
{
    public class GetSuccessStoriesQuery:IRequest<Result<IReadOnlyList<PublicCampaignDto>>>
    {
        public string? AcceptLanguage { get; set; }
    }
}
