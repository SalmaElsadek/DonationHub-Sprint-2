using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Infrastructure.Identity.DTOs;
using DonationHub.Infrastructure.Identity.Services;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthDTO>>
    {
        private readonly IAuthenticationService _authentication;

        public RegisterCommandHandler(IAuthenticationService authentication)
        {
            _authentication = authentication;
        }
        public async Task<Result<AuthDTO>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = new RegisterDTO { 
                FullName = request.FullName,
                Email = request.Email,
                Password = request.Password,
                PreferredLanguage=request.PreferredLanguage
            };

            return await _authentication.RegisterAsync(result,cancellationToken);
        }
    }
}
