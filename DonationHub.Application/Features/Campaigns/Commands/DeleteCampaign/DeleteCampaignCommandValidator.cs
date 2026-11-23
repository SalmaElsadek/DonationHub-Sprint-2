using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DonationHub.Application.Features.Campaigns.Commands.DeleteCampaign
{
    public class DeleteCampaignCommandValidator:AbstractValidator<DeleteCampaignCommand>
    {
        public DeleteCampaignCommandValidator()
        {
            RuleFor(x=>x.Id).NotEmpty()
                .WithMessage("Campaign Id is Required");
           
        }
    }
}
