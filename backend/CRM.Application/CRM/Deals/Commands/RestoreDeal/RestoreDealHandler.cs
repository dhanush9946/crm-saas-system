using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Customers.Interfaces;
using CRM.Application.CRM.Deals.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Deals.Commands.RestoreDeal;

public sealed class RestoreDealHandler
    : IRequestHandler<RestoreDealCommand>
{
    private readonly IDealRepository _dealRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RestoreDealHandler(
        IDealRepository dealRepository,
        ICustomerRepository customerRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _dealRepository = dealRepository;
        _customerRepository = customerRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        RestoreDealCommand request,
        CancellationToken cancellationToken)
    {
        var deal = await _dealRepository.GetDeletedByIdAsync(
            _currentUser.TenantId,
            request.DealId,
            cancellationToken);

        if (deal is null)
        {
            throw new NotFoundException(
                $"Deal '{request.DealId}' was not found.");
        }

        var customer = await _customerRepository.GetByIdAsync(
            _currentUser.TenantId,
            deal.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException(
                $"Customer '{deal.CustomerId}' was not found.");
        }

        deal.Restore();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}
