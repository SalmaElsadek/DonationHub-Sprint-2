using DonationHub.Application.DTOs;
using DonationHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DonationHub.Api.Common.Results;

namespace DonationHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _categoryService.GetAllAsync(ct);
            return result.ToActionResult();
        }

        // GET: api/categories/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result= await _categoryService.GetByIdAsync(id,ct);
            return result.ToActionResult();
        }

        // POST: api/categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryDTO categoryDTO, CancellationToken ct)
        {
           
            var result = await _categoryService.CreateAsync(categoryDTO,ct);
            return result.ToActionResult(c => CreatedAtAction(nameof(GetById), new { id = c.Id }, c));
        }

        // PUT: api/categories/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDTO categoryDto, CancellationToken ct)
        {
            var result = await _categoryService.GetByIdAsync(id, ct);
            return result.ToActionResult(StatusCodes.Status204NoContent);
        }

        // DELETE: api/categories/{id} 
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _categoryService.DeleteAsync(id, ct);
            return result.ToActionResult(StatusCodes.Status204NoContent);
        }
    }
}
