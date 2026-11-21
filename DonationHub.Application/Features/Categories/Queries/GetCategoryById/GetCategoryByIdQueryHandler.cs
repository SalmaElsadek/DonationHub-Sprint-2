using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.DTOs;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDTO>>
    {
        private readonly IGenericRepository<Category> _generic;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(IGenericRepository<Category> generic,IMapper mapper)
        {
            _generic = generic;
            _mapper = mapper;
        }
        public async Task<Result<CategoryDTO>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category=await _generic.GetByIdAsync(request.Id,cancellationToken);

            if(category is null) return CategoryErrors.NotFound;

            return Result.Success(_mapper.Map<CategoryDTO>(category));
        }
    }
}
