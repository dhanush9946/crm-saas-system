using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Deals.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Deals.Commands.DeleteDeal;

public sealed class DeleteDealHandler
    : IRequestHandler<DeleteDealCommand>
{
    private readonly IDealRepository _dealRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDealHandler(
        IDealRepository dealRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _dealRepository = dealRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteDealCommand request,
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

        deal.SoftDelete();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
