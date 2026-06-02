using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Customers.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Customers.Commands.RestoreCustomer;

public sealed class RestoreCustomerHandler
    : IRequestHandler<RestoreCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RestoreCustomerHandler(
        ICustomerRepository customerRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        RestoreCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetDeletedByIdAsync(
                                _currentUser.TenantId,
                                request.CustomerId,
                                cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException(
                $"Customer '{request.CustomerId}' was not found.");
        }

        customer.Restore();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}