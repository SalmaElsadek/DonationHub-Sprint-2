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

namespace DonationHub.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<IEnumerable<CategoryDTO>>>
    {
        private readonly IGenericRepository<Category> _generic;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(IGenericRepository<Category> generic,IMapper mapper)
        {
            _generic = generic;
            _mapper = mapper;
        }
        public async Task<Result<IEnumerable<CategoryDTO>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories=await _generic.GetAllAsync(cancellationToken);
            return Result.Success(_mapper.Map<IEnumerable<CategoryDTO>>(categories));
        }
    }
}
