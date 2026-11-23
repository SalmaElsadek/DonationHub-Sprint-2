using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DonationHub.Application.Features.Campaigns.Commands.CreateCampaign
{
    public class CreateCampaignCommandValidator:AbstractValidator<CreateCampaignCommand>
    {
        public CreateCampaignCommandValidator()
        {
          
            RuleFor(x => x.TitleAr).NotEmpty()
                .WithMessage("Arabic title is required")
                .MaximumLength(200).WithMessage("Arabic title must not exceed 200 characters");

            RuleFor(x => x.TitleEn).NotEmpty()
                .WithMessage("English title is required")
                .MaximumLength(200).WithMessage("English title must not exceed 200 characters");

            RuleFor(x => x.DescriptionAr).NotEmpty()
                .WithMessage("Arbic description is required");

            RuleFor(x => x.DescriptionEn).NotEmpty()
                .WithMessage("English description is required");

            RuleFor(x => x.GoalAmount)
                .GreaterThan(0).WithMessage("Goal amount must be greater than zero");

            RuleFor(x => x.GoalCurrency)
                .NotEmpty().WithMessage("Goal currency is required.")
                .Length(3).WithMessage("Goal currency must be a 3-letter code");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required");

            RuleFor(x => x.EndDate).NotEmpty()
                .WithMessage("End Date is Required");
               
        }
    }
}
