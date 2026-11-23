using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace DonationHub.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator:AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.NameAr)
                .NotEmpty().WithMessage("Arabic category name is required")
                .MaximumLength(150).WithMessage("Arabic category name must not exceed 150 characters");

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage("English category name is required")
                .MaximumLength(150).WithMessage("English category name must not exceed 150 characters");
        }
    }
}
