using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using DonationHub.Domain.ValueObjects;
using MediatR;
using Microsoft.VisualBasic;

namespace DonationHub.Application.Features.Campaigns.Commands.CreateCampaign
{
    public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, Result<AdminCampaignDto>>
    {
        private readonly IGenericRepository<Campaign> _campaignRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public CreateCampaignCommandHandler(IGenericRepository<Campaign> campaignRepo,IGenericRepository<Category> categoryRepo,IUnitOfWork unitOfWork,TimeProvider timeProvider)
        {
            _campaignRepo = campaignRepo;
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }
        public async Task<Result<AdminCampaignDto>> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
        {
            var time = _timeProvider.GetUtcNow();
            var category=await _categoryRepo.GetByIdAsync(request.CategoryId,cancellationToken);
            if (category is null)
                return Result.Failure<AdminCampaignDto>(CategoryErrors.NotFound);

            if (request.EndDate <= time)
                return Result.Failure<AdminCampaignDto>(CampaignErrors.EndDateInPast);

            var money = Money.Create(request.GoalAmount, request.GoalCurrency);

            var campaign = Campaign.Create(
                request.TitleAr,
                request.TitleEn,
                request.DescriptionAr,
                request.DescriptionEn,
                money,
                request.EndDate,
                request.CategoryId,
                request.CreatedByUserId,
                time,
                request.BannerFileName
                );

            await _campaignRepo.AddAsync(campaign, cancellationToken);
            await _unitOfWork.CompeleteAsync(cancellationToken);

            var dayLeft =(campaign.EndDate - time).Days;

            var dto = new AdminCampaignDto
            {
                Id = campaign.Id,
                TitleAr = campaign.TitleAr,
                TitleEn = campaign.TitleEn,
                DescriptionAr = campaign.DescriptionAr,
                DescriptionEn = campaign.DescriptionEn,
                GoalAmount = campaign.GoalAmount.Amount,
                GoalCurrency = campaign.GoalAmount.Currency,
                RaisedAmount = campaign.RaisedAmount,
                EndDate = campaign.EndDate,
                CategoryId = campaign.CategoryId,
                BannerFileName = campaign.BannerFileName,
                Status = campaign.Status.ToString(),
                CreatedByUserId = campaign.CreatedByUserId,
                UpdatedAt = campaign.UpdatedAt,
                RowVersion = campaign.RowVersion,
                ProgressPercent = 0,
                DaysLeft = dayLeft
            };

            return Result.Success( dto );
        }
    }
}
