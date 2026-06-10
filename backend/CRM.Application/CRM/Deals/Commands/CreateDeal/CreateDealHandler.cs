using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Customers.Interfaces;
using CRM.Application.CRM.Deals.Interfaces;
using CRM.Domain.CRM.Entities;
using MediatR;

namespace CRM.Application.CRM.Deals.Commands.CreateDeal;

public sealed class CreateDealHandler
    : IRequestHandler<CreateDealCommand, Guid>
{
    private readonly IDealRepository _dealRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDealHandler(
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

    public async Task<Guid> Handle(
        CreateDealCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException(
                $"Customer '{request.CustomerId}' was not found.");
        }

        var deal = Deal.Create(
            tenantId: _currentUser.TenantId,
            title: request.Title,
            customerId: request.CustomerId,
            leadId: request.LeadId,
            value: request.Value,
            stage: request.Stage,
            expectedCloseDate: request.ExpectedCloseDate,
            ownerUserId: request.OwnerUserId);

        await _dealRepository.AddAsync(
            deal,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return deal.Id;
    }
}
