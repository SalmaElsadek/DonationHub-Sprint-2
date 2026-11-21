using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DonationHub.Application.Common.Results;
using DonationHub.Application.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDTO>>
    {
        private readonly IGenericRepository<Category> _generic;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public CreateCategoryCommandHandler(IGenericRepository<Category> generic,IMapper mapper,
            IUnitOfWork unitOfWork,TimeProvider timeProvider)
        {
            _generic = generic;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }
        public async Task<Result<CategoryDTO>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var time=_timeProvider.GetUtcNow();

            var category = Category.Create(
                request.NameAr,
                request.NameEn,
                request.DescriptionAr,
                request.DescriptionEn,
                time
                );

            await _generic.AddAsync(category,cancellationToken);
            await _unitOfWork.CompeleteAsync(cancellationToken);

            return Result.Success(_mapper.Map<CategoryDTO>(category));
        }
    }
}
