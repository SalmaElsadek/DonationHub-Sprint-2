using System.Security.Claims;
using DonationHub.Api.Common.Results;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Features.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.Features.Campaigns.Commands.DeleteCampaign;
using DonationHub.Application.Features.Campaigns.Commands.UpdateCampaign;
using DonationHub.Application.Features.Campaigns.Queries.GetActiveCampaigns;
using DonationHub.Application.Features.Campaigns.Queries.GetCampaignById;
using DonationHub.Application.Features.Campaigns.Queries.GetSuccessStories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CampaignsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //Get /api/campaigns
        [HttpGet]
        public async Task<IActionResult> GetActiveCampaigns([FromHeader(Name = "Accept-Language")] string? acceptLanguage,
            CancellationToken ct)
        {
            var query = new GetActiveCampaignsQuery
            {
                AcceptLanguage = acceptLanguage
            };
            var result = await _mediator.Send(query, ct);
            return HandleResult(result);
        }

        //Get /api/campaigns/success-stories
        [HttpGet("success-stories")]
        public async Task<IActionResult> GetSuccessStories(
            [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
            CancellationToken ct)
        {
            var query = new GetSuccessStoriesQuery
            {
                AcceptLanguage = acceptLanguage
            };

            var result = await _mediator.Send(query, ct);
            return HandleResult(result);
        }

        // Get /api/campaigns/{id:guid}
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(
            Guid id,
            [FromHeader(Name = "Accept-Language")] string? acceptLanguage,
            CancellationToken ct)
        {
            //admin responses Bilingual
            var isAdmin = User.Identity?.IsAuthenticated == true && User.IsInRole("Admin");

            var query = new GetCampaignByIdQuery
            {
                Id = id,
                AcceptLanguage = acceptLanguage,
                IncludeBilingualDetails = isAdmin
            };

            var result = await _mediator.Send(query, ct);
            return HandleResult(result);
        }

        //// Post /api/campaigns
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateCampaignCommand command,
            CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            //بتعمل نسخة جديدة منها ومتعدل فيها CreatedByUserId
            //with=>Record
            var commandWithUser = command with { CreatedByUserId = userId };

            var result = await _mediator.Send(commandWithUser, ct);
            if (result.IsFailure)
                return result.ToProblemDetails(HttpContext);
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value.Id },
                result.Value);
        }

        // Put /api/campaigns/{id:guid}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateCampaignCommand command,
            CancellationToken ct)
        {
            if (id != command.Id)
                return BadRequest("Route ID does not match body ID.");

            var result = await _mediator.Send(command, ct);
            return HandleResult(result);
        }

        // Delete /api/campaigns/{id:guid}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken ct)
        {
            var command = new DeleteCampaignCommand(id);
            var result = await _mediator.Send(command, ct);
            return HandleResult(result);
        }

        private IActionResult HandleResult<T>(Result<T> result)
            => result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails(HttpContext);

        private IActionResult HandleResult(Result result)
            => result.IsSuccess ? NoContent() : result.ToProblemDetails(HttpContext);
    }
}
