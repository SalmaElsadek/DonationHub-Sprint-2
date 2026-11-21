using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Infrastructure.Identity.DTOs;
using DonationHub.Infrastructure.Identity.Services;
using MediatR;

namespace DonationHub.Application.Features.Auth.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserProfileDTO>>
    {
        private readonly IAuthenticationService _authentication;

        public GetCurrentUserQueryHandler(IAuthenticationService authentication)
        {
            _authentication = authentication;
        }
        public async Task<Result<UserProfileDTO>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            return await _authentication.GetUserProfileAsync(request.Id, cancellationToken);
        }
    }
}
