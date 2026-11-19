using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Options;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Data;
using DonationHub.Infrastructure.Identity.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using DonationHub.Application.Common.Errors;
namespace DonationHub.Infrastructure.Identity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly AppDbContext _context;
        private readonly TimeProvider _timeProvider;
        private readonly JwtOptions _options;

        public AuthenticationService(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole<Guid>>roleManager
            ,AppDbContext context,IOptions<JwtOptions>options, TimeProvider timeProvider)
        {
           _userManager = userManager;
           _roleManager = roleManager;
           _context = context;
           _timeProvider = timeProvider;
            _options = options.Value;
        }
        public async Task<Result<bool>> CreateStaffAsync(StaffDTO model, CancellationToken ct = default)
        {
            if (model.Role != "Admin" && model.Role != "CampaignManager")
                return Error.Validation("Auth.InvalidStaffRole", "Invalid staff role specified.");

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                IsActive = true,
                EmailConfirmed=true
            };

            var result=await _userManager.CreateAsync(user,model.Password);

            if (!result.Succeeded){
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return Error.Validation("Auth.StaffCreationFailed", errorMessage);
            }
            await _userManager.AddToRoleAsync(user, model.Role);
            return true;
        }

        public async Task<Result<UserProfileDTO>> GetUserProfileAsync(Guid userId, CancellationToken ct = default)
        {
            var user=await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || !user.IsActive)
            {
                return Error.NotFound("Auth.UserNotFound", "The requested user was not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new UserProfileDTO
            {
                UserID = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PreferredLanguage = user.PreferredLanguage,
                Roles = roles
            };
        }

        public async Task<Result<AuthDTO>> LoginAsync(LoginDTO model, CancellationToken ct = default)
        {
            var user=await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password)) {
                return AuthenticationErrors.InvalidCredentials;
            }

            if (!user.IsActive) {
                return AuthenticationErrors.AccountDeactivated;
            }
            return await GenerateAuthResponseAsync(user,ct);
        }

        public async Task<Result<AuthDTO>> RefreshTokenAsync(RefreshTokenDTO model, CancellationToken ct = default)
        {
            var time=_timeProvider.GetUtcNow();
            var tokenHash=HashToken(model.RefreshToken);

            var storedToken=await _context.RefreshTokens
                .FirstOrDefaultAsync(r=>r.TokenHash == tokenHash);

            if (storedToken == null || !storedToken.IsActive || storedToken.ExpiredAt <= time)
            {
                return AuthenticationErrors.RefreshTokenInvalid;
            }

            var user=await _userManager.FindByIdAsync(storedToken.UserId.ToString());
            if (user == null || !user.IsActive)
            {
                return AuthenticationErrors.AccountDeactivated;
            }

            storedToken.Revoke();
            await _context.SaveChangesAsync();
            return await GenerateAuthResponseAsync(user);
        }

        public async Task<Result<AuthDTO>> RegisterAsync(RegisterDTO model, CancellationToken ct = default)
        {
            var existingUser=await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return AuthenticationErrors.EmailAlreadyExists;
            }
                var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PreferredLanguage = string.IsNullOrEmpty(model.PreferredLanguage) ? "en" : model.PreferredLanguage,
                IsActive = true
            };

            var result=await _userManager.CreateAsync(user,model.Password);
            if (!result.Succeeded)
            {
                var error=string.Join(", ",result.Errors.Select(e=>e.Description));
                return Error.Validation("Auth.RegistrationFailed", error);
            }

            await _userManager.AddToRoleAsync(user, "Donor");

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<Result> RevokeTokenAsync(string token, CancellationToken ct = default)
        {
            var tokenHash=HashToken(token);
            var refreshToken=await _context.RefreshTokens.FirstOrDefaultAsync(r=>r.TokenHash==tokenHash);

            if(refreshToken==null||!refreshToken.IsActive)
                return Result.Failure(AuthenticationErrors.RefreshTokenInvalid);

            refreshToken.Revoke();
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        //Helper Method يارب ما نبص فيها تاني
        private string HashToken(string token)
        {
            using var sha256=SHA256.Create(); 
            var hashByte=sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashByte);
        }

        private sealed record GeneratedRefreshToken(RefreshToken Entity, string RawToken);
        private GeneratedRefreshToken GenerateRefreshToken(Guid userId, DateTimeOffset time)
        {
            var randomNumber = new byte[64];
            //old
            //using var generator = new RNGCryptoServiceProvider();

            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            var tokenString = Convert.ToBase64String(randomNumber);
            var tokenHash = HashToken(tokenString);

            var entity = RefreshToken.Create(
                userId,
                tokenHash,
                time.AddDays(_options.RefreshTokenExpiryDays));

            return new GeneratedRefreshToken(entity, tokenString);
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user,DateTimeOffset time)
        {
            var claims = new List<Claim> { 
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email??string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())            
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = time.UtcDateTime.AddMinutes(_options.ExpiryMinutes),
                Issuer = _options.Issuer,
                Audience =_options.Audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

        private async Task<AuthDTO> GenerateAuthResponseAsync(ApplicationUser user, CancellationToken ct = default)
        {
            var time=_timeProvider.GetUtcNow();
            var refreshToken = GenerateRefreshToken(user.Id,time);
            _context.RefreshTokens.Add(refreshToken.Entity);
            await _context.SaveChangesAsync();

            var accessToken = await GenerateJwtTokenAsync(user, time);
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthDTO
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                AccessToken = accessToken,
                AccessTokenExpiration = time.AddMinutes(_options.ExpiryMinutes),
                RefreshToken = refreshToken.RawToken,
                RefreshTokenExpiration = refreshToken.Entity.ExpiredAt.UtcDateTime,
                Roles = roles.ToList()
            };
        }
    }
}
