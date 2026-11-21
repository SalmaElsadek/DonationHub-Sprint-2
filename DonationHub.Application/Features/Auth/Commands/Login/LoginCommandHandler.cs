using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Infrastructure.Identity.DTOs;
using DonationHub.Infrastructure.Identity.Services;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthDTO>>
    {
        private readonly IAuthenticationService _authentication;

        public LoginCommandHandler(IAuthenticationService authentication)
        {
            _authentication = authentication;
        }
        public async Task<Result<AuthDTO>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = new LoginDTO { 
                Email=request.Email,
                Password=request.Password,
            };

            return await _authentication.LoginAsync(result, cancellationToken);
        }
    }
}
