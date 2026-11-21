using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Infrastructure.Identity.DTOs;
using DonationHub.Infrastructure.Identity.Services;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthDTO>>
    {
        private readonly IAuthenticationService _authentication;

        public RefreshTokenCommandHandler(IAuthenticationService authentication)
        {
            _authentication = authentication;
        }
        public async Task<Result<AuthDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = new RefreshTokenDTO { 
                RefreshToken= request.RefreshToken,
            };
            return await _authentication.RefreshTokenAsync(result,cancellationToken);
        }
    }
}
