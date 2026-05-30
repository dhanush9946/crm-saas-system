using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Customers.Interfaces;
using CRM.Domain.CRM.Entities;
using MediatR;

namespace CRM.Application.CRM.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerHandler
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(
        ICustomerRepository customerRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _customerRepository.ExistsAsync(
            _currentUser.TenantId,
            request.Name,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "Customer with the same name already exists.");
        }

        var customer = Customer.Create(
            tenantId: _currentUser.TenantId,
            name: request.Name,
            industry: request.Industry,
            website: request.Website,
            ownerUserId: request.OwnerUserId);

        await _customerRepository.AddAsync(
            customer,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return customer.Id;
    }
}