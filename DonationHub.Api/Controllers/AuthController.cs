using DonationHub.Api.Common.Results;
using DonationHub.Application.Features.Auth.Commands.Login;
using DonationHub.Application.Features.Auth.Commands.RefreshToken;
using DonationHub.Application.Features.Auth.Commands.Register;
using DonationHub.Application.Features.Auth.Commands.RevokeToken;
using DonationHub.Application.Features.Auth.Queries.GetCurrentUser;
using DonationHub.Infrastructure.Identity.DTOs;
using DonationHub.Infrastructure.Identity.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        //private readonly IAuthenticationService _service;

        //public AuthController(IAuthenticationService service)
        //{
        //    _service = service;
        //}

        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model, CancellationToken ct)
        {

            var command = new RegisterCommand(
                            model.FullName,
                            model.Email,
                            model.Password,
                            model.PreferredLanguage);

            var result = await _mediator.Send(command, ct);
            return result.ToActionResult();
              
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model, CancellationToken ct)
        {

            var command = new LoginCommand(model.Email, model.Password);
            var result = await _mediator.Send(command, ct);
            return result.ToActionResult();
            
        }

        [HttpPost("refresh_token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO model, CancellationToken ct)
        {
            var command = new RefreshTokenCommand(model.RefreshToken);
            var result = await _mediator.Send(command, ct);
            return result.ToActionResult();
        }

        [HttpPost("revoke_token")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDTO model, CancellationToken ct)
        {
            var command = new RevokeTokenCommand(model.RefreshToken);
            var revoked = await _mediator.Send(command, ct);
            return revoked.ToActionResult();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var profile = await _mediator.Send(new GetCurrentUserQuery(userId), ct);
            return profile.ToActionResult();
        }
    }
}
