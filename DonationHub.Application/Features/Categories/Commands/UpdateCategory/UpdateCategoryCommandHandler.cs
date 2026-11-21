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

namespace DonationHub.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
    {
        private readonly IGenericRepository<Category> _generic;
        private readonly TimeProvider _timeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryCommandHandler(IGenericRepository<Category> generic,TimeProvider timeProvider,IUnitOfWork unitOfWork)
        {
            _generic = generic;
            _timeProvider = timeProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var time=_timeProvider.GetUtcNow();

            var category=await _generic.GetByIdAsync(request.Id,cancellationToken);
            if (category==null) return Result.Failure(CategoryErrors.NotFound);

            category.UpdateCreate(
                request.NameAr,
                request.NameEn,
                request.DescriptionAr,
                request.DescriptionEn,
                time
                );

            _generic.Update(category);
            await _unitOfWork.CompeleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}
