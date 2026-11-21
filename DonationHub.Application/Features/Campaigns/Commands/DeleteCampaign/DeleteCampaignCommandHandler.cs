using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Interfaces;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Commands.DeleteCampaign
{
    public class DeleteCampaignCommandHandler : IRequestHandler<DeleteCampaignCommand, Result>
    {
        private readonly IGenericRepository<Campaign> _generic;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCampaignCommandHandler(IGenericRepository<Campaign>generic,IUnitOfWork unitOfWork)
        {
            _generic = generic;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(DeleteCampaignCommand request, CancellationToken cancellationToken)
        {
            var result = await _generic.GetByIdAsync(request.Id, cancellationToken);
            if (result == null)
                return Result.Failure(CampaignErrors.NotFound);

            await _generic.Remove(result.Id, cancellationToken);
            await _unitOfWork.CompeleteAsync(cancellationToken);

            return Result.Success();
        }
    }
}
