using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Features.Campaigns.DTOs;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Queries.GetCampaignById
{
    public class GetCampaignByIdQuery : IRequest<Result<CampaignDetailsDto>>
    {
        public Guid Id { get; set; }
        public string? AcceptLanguage { get; set; }
        public bool IncludeBilingualDetails { get; set; }
    }
}
