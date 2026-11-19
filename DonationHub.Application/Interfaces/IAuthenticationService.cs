using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Infrastructure.Identity.DTOs;

namespace DonationHub.Infrastructure.Identity.Services
{
    public interface IAuthenticationService
    {
        Task<Result<AuthDTO>> RegisterAsync(RegisterDTO model, CancellationToken ct = default);
        Task<Result<AuthDTO>> LoginAsync(LoginDTO model, CancellationToken ct = default);
        Task<Result<AuthDTO>> RefreshTokenAsync(RefreshTokenDTO model, CancellationToken ct = default);
        Task<Result> RevokeTokenAsync(string refreshToken, CancellationToken ct = default);
        Task<Result<UserProfileDTO>> GetUserProfileAsync(Guid userId, CancellationToken ct = default);
        Task<Result<bool>> CreateStaffAsync(StaffDTO model,CancellationToken ct=default);
    }
}
