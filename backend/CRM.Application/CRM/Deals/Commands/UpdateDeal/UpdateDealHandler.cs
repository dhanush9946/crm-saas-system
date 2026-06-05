using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Deals.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Deals.Commands.UpdateDeal;

public sealed class UpdateDealHandler
    : IRequestHandler<UpdateDealCommand>
{
    private readonly IDealRepository _dealRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConcurrencyService _concurrencyService;

    public UpdateDealHandler(
        IDealRepository dealRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        IConcurrencyService concurrencyService)
    {
        _dealRepository = dealRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _concurrencyService = concurrencyService;
    }

    public async Task Handle(
        UpdateDealCommand request,
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

        var rowVersion =
            Convert.FromBase64String(
                request.RowVersion);

        _concurrencyService.SetOriginalRowVersion(
            deal,
            rowVersion);

        deal.Update(
            request.Title,
            request.Value,
            request.ExpectedCloseDate,
            request.OwnerUserId);

        try
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (ConcurrencyException)
        {
            throw new ConcurrencyException(
                "The deal was modified by another user. Please refresh and try again.");
        }
    }
}
