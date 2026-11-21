using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Application.DTOs;
using MediatR;

namespace DonationHub.Application.Features.Categories.Queries.GetAllCategories
{
    public record GetAllCategoriesQuery(): IRequest<Result<IEnumerable<CategoryDTO>>>;
}
