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
    public record GetCurrentUserQuery(Guid Id) :IRequest<Result<UserProfileDTO>>;
}
