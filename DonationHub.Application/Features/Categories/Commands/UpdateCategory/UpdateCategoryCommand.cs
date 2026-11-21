using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Features.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(Guid Id, string NameAr, string NameEn, string? DescriptionAr, string? DescriptionEn)
        : IRequest<Result>;

}
