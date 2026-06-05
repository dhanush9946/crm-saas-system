using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Deals.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Deals.Commands.ChangeDealStage;

public sealed class ChangeDealStageHandler
    : IRequestHandler<ChangeDealStageCommand>
{
    private readonly IDealRepository _dealRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeDealStageHandler(
        IDealRepository dealRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _dealRepository = dealRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ChangeDealStageCommand request,
        CancellationToken cancellationToken)
    {
        var deal = await _dealRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.DealId,
            cancellationToken);

        if (deal is null)
        {
            throw new NotFoundException(
                $"Deal '{request.DealId}' was not found.");
        }

        deal.ChangeStage(
            request.Stage);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
