using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Infrastructure.Identity.DTOs;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(string FullName,
        string Email, string Password, string? PreferredLanguage)
        :IRequest<Result<AuthDTO>>;
}
