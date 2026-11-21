using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Campaign> _campaignRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryCommandHandler(IGenericRepository<Category> categoryRepo,IGenericRepository<Campaign> campaignRepo,IUnitOfWork unitOfWork)
        {
            _categoryRepo = categoryRepo;
            _campaignRepo = campaignRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category =await _categoryRepo.GetByIdAsync(request.Id,cancellationToken);

            if (category == null) return Result.Failure(CategoryErrors.NotFound);

            var hasCampaign = await _campaignRepo.Query().IgnoreQueryFilters()
                .AnyAsync(c => c.CategoryId == request.Id, cancellationToken);

            if (hasCampaign) return Result.Failure(CategoryErrors.HasCampaigns);

            await _categoryRepo.Remove(request.Id,cancellationToken);
            await _unitOfWork.CompeleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}
