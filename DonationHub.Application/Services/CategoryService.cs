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
using Microsoft.EntityFrameworkCore;
namespace DonationHub.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _generic;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Campaign> _campaignRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public CategoryService(IGenericRepository<Category> generic,IMapper mapper,IGenericRepository<Campaign> campaignRepo,IUnitOfWork unitOfWork,TimeProvider timeProvider)
        {
             _generic = generic;
            _mapper = mapper;
            _campaignRepo = campaignRepo;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }
        public async Task<Result<CategoryDTO>> CreateAsync(CategoryDTO model, CancellationToken ct = default)
        {
           var time= _timeProvider.GetUtcNow();
            var result = Category.Create(
                model.NameAr,
                model.NameEn,
                model.DescriptionAr,
                model.DescriptionEn,
                time
                );
            await _generic.AddAsync(result,ct);
            await _unitOfWork.CompeleteAsync();
            return _mapper.Map<CategoryDTO>(result);
        }

        public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var result= await _generic.GetByIdAsync(id,ct);
            if (result == null) return Result.Failure(CategoryErrors.NotFound);


            var hasCampaign = await _campaignRepo.Query().IgnoreQueryFilters()
                            .AnyAsync(c => c.CategoryId == id, ct);

            if (hasCampaign) throw new InvalidOperationException("Category.HasCampaigns"); //409 Conflict

            await _generic.Remove(id);
            await _unitOfWork.CompeleteAsync();

            return Result.Success();
        }

        public async Task<Result<IEnumerable<CategoryDTO>>> GetAllAsync(CancellationToken ct = default)
        {
            var result = await _generic.GetAllAsync(ct);
            return Result.Success(_mapper.Map<IEnumerable<CategoryDTO>>(result));
        }

        public async Task<Result<CategoryDTO>> GetByIdAsync(Guid id, CancellationToken ct = default)        {
            var result= await _generic.GetByIdAsync(id,ct);
            if (result == null) return CategoryErrors.NotFound;

            return _mapper.Map<CategoryDTO>(result);
        }

        public async Task<Result> UpdateAsync(Guid id, CategoryDTO model, CancellationToken ct = default)
        {
            var result = await _generic.GetByIdAsync(id,ct);
            if (result == null) return Result.Failure(CategoryErrors.NotFound);

            var time=_timeProvider.GetUtcNow();
            result.UpdateCreate(
                model.NameAr,
                model.NameEn,
                model.DescriptionAr,
                model.DescriptionEn,
                time
                );
            _generic.Update(result);
            await _unitOfWork.CompeleteAsync();
            return Result.Success();
        }
    }
}
