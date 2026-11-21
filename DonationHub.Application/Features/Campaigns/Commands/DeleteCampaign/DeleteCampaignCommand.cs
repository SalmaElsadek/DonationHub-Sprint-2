using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Commands.DeleteCampaign
{
    public record DeleteCampaignCommand(Guid Id):IRequest<Result>;
   
}
