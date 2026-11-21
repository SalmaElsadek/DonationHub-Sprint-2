using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Application.DTOs;
using MediatR;

namespace DonationHub.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string NameAr,string NameEn,string? DescriptionAr,string? DescriptionEn)
        :IRequest<Result<CategoryDTO>>;
    
}
