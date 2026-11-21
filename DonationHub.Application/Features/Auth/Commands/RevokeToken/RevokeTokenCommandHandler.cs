using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Infrastructure.Identity.Services;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.RevokeToken
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Result>
    {
        private readonly IAuthenticationService _authentication;

        public RevokeTokenCommandHandler(IAuthenticationService authentication)
        {
            _authentication = authentication;
        }
        public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authentication.RevokeTokenAsync(request.RefreshToken, cancellationToken);
        }
    }
}
