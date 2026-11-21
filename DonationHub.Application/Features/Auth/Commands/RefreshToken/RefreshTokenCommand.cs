using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Results;
using DonationHub.Infrastructure.Identity.DTOs;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken):IRequest<Result<AuthDTO>>;
}
