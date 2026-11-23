using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DonationHub.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryValidator:AbstractValidator<GetCategoryByIdQuery>
    {

        public GetCategoryByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Category Id is required");
        }
    }
}
