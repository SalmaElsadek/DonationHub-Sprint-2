using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enum;
using DonationHub.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Campaigns.Commands.UpdateCampaign
{
    public class UpdateCampaignCommandHandler : IRequestHandler<UpdateCampaignCommand, Result>
    {
        private readonly IGenericRepository<Campaign> _campaignRepo;
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;
        private readonly ICampaignConcurrencyService _campaignConcurrency;

        public UpdateCampaignCommandHandler(IGenericRepository<Campaign> campaignRepo, IGenericRepository<Category> categoryRepo
            , IUnitOfWork unitOfWork, TimeProvider timeProvider,ICampaignConcurrencyService campaignConcurrency)
        {
            _campaignRepo = campaignRepo;
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
            _campaignConcurrency = campaignConcurrency;
        }
        public async Task<Result> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
        {
            var campaign = await _campaignRepo.GetByIdAsync(request.Id, cancellationToken);
            if (campaign is null)
                return Result.Failure(CampaignErrors.NotFound);

            if (campaign.Status is CampaignStatus.Completed or CampaignStatus.Expired)
                return Result.Failure(CampaignErrors.NotActive);

            var category = await _categoryRepo.GetByIdAsync(request.CategoryId, cancellationToken);
            if (category is null)
                return Result.Failure(CategoryErrors.NotFound);

            var time = _timeProvider.GetUtcNow();
            if (request.EndDate <= time)
                return Result.Failure(CampaignErrors.EndDateInPast);

            if (request.GoalAmount < campaign.RaisedAmount)
                return Result.Failure(CampaignErrors.GoalBelowRaised);

            _campaignConcurrency.SetOriginalRowVersion(campaign, request.RowVersion);
            var money = Money.Create(request.GoalAmount, request.GoalCurrency);
            campaign.Update(
                request.TitleAr,
                request.TitleEn,
                request.DescriptionAr,
                request.DescriptionEn,
                money,
                time,
                request.CategoryId,
                time
                );
            campaign.UpdateEndDate(time, request.EndDate);
            campaign.UpdateGoalAmount(money, time);
            _campaignRepo.Update(campaign);

            try
            {
                await _unitOfWork.CompeleteAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Failure(CampaignErrors.ConcurrencyConflict);
            }

            return Result.Success();
        }
    }
}
