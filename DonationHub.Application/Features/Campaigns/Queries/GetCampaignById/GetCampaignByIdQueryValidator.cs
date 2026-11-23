using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DonationHub.Application.Features.Campaigns.Queries.GetCampaignById
{
    public class GetCampaignByIdQueryValidator:AbstractValidator<GetCampaignByIdQuery>
    {
        public GetCampaignByIdQueryValidator() {
            RuleFor(x=>x.Id).NotEmpty()
                .WithMessage("Campaign Id is Required");
        }
    }
}
