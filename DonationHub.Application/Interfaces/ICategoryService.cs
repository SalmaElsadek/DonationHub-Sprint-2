using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Application.DTOs;

namespace DonationHub.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<IEnumerable<CategoryDTO>>> GetAllAsync(CancellationToken ct = default);
        Task<Result<CategoryDTO>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Result<CategoryDTO>> CreateAsync(CategoryDTO model, CancellationToken ct = default);
        Task<Result> UpdateAsync(Guid id, CategoryDTO model, CancellationToken ct = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
