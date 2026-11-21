using DonationHub.Api.Common.Results;
using DonationHub.Application.DTOs;
using DonationHub.Application.Features.Categories.Commands.CreateCategory;
using DonationHub.Application.Features.Categories.Commands.DeleteCategory;
using DonationHub.Application.Features.Categories.Commands.UpdateCategory;
using DonationHub.Application.Features.Categories.Queries.GetAllCategories;
using DonationHub.Application.Features.Categories.Queries.GetCategoryById;
using DonationHub.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DonationHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        //private readonly ICategoryService _categoryService;

        //public CategoriesController(ICategoryService categoryService)
        //{
        //    _categoryService = categoryService;
        //}

        private readonly IMediator _mediator;
        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAllCategoriesQuery(), ct);
            return result.ToActionResult();
        }

        // GET: api/categories/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id), ct);
            return result.ToActionResult();
        }

        // POST: api/categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryDTO categoryDTO, CancellationToken ct)
        {
            var command = new CreateCategoryCommand(
                categoryDTO.NameAr,
                categoryDTO.NameEn,
                categoryDTO.DescriptionAr,
                categoryDTO.DescriptionEn
                );
            var result = await _mediator.Send(command, ct);
            return result.ToActionResult(c => CreatedAtAction(nameof(GetById), new { id = c.Id }, c));
        }

        // PUT: api/categories/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDTO categoryDto, CancellationToken ct)
        {
            var command = new UpdateCategoryCommand(
                id,
                categoryDto.NameAr,
                categoryDto.NameEn,
                categoryDto.DescriptionAr,
                categoryDto.DescriptionEn);

            var result = await _mediator.Send(command, ct);
            return result.ToActionResult(StatusCodes.Status204NoContent);
        }

        // DELETE: api/categories/{id} 
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id), ct);
            return result.ToActionResult(StatusCodes.Status204NoContent);
        }
    }
}
