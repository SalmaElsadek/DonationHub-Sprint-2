using DonationHub.Api.Common.Results;
using DonationHub.Infrastructure.Identity.DTOs;
using DonationHub.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _service;

        public AuthController(IAuthenticationService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model, CancellationToken ct)
        {
           
             var result = await _service.RegisterAsync(model,ct);
            return result.ToActionResult();
              
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model, CancellationToken ct)
        {
           
            var result = await _service.LoginAsync(model,ct);
            return result.ToActionResult();
            
        }

        [HttpPost("refresh_token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO model, CancellationToken ct)
        {
           var result=await _service.RefreshTokenAsync(model,ct);
           return result.ToActionResult();
        }

        [HttpPost("revoke_token")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDTO model, CancellationToken ct)
        {
            var revoked = await _service.RevokeTokenAsync(model.RefreshToken);
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

            var profile = await _service.GetUserProfileAsync(userId,ct);
            return profile.ToActionResult();
        }
    }
}
