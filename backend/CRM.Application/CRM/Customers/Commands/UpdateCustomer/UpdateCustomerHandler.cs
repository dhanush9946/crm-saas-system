using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Customers.Interfaces;
using MediatR;


namespace CRM.Application.CRM.Customers.Commands.UpdateCustomer;

public sealed class UpdateCustomerHandler
    : IRequestHandler<UpdateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConcurrencyService _concurrencyService;

    public UpdateCustomerHandler(
        ICustomerRepository customerRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        IConcurrencyService concurrencyService)
    {
        _customerRepository = customerRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _concurrencyService = concurrencyService;
    }

    public async Task Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        //-----------------------------------------
        // Get Customer
        //-----------------------------------------

        var customer = await _customerRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException(
                $"Customer '{request.CustomerId}' was not found.");
        }

        //-----------------------------------------
        // Duplicate Name Check
        //-----------------------------------------

        var duplicateExists =
            await _customerRepository
                .CustomerNameExistsForOtherCustomerAsync(
                    _currentUser.TenantId,
                    request.CustomerId,
                    request.Name,
                    cancellationToken);

        if (duplicateExists)
        {
            throw new ConflictException(
                "Customer with the same name already exists.");
        }

        //-----------------------------------------
        // Apply Original RowVersion
        //-----------------------------------------

        var rowVersion =
            Convert.FromBase64String(
                request.RowVersion);

        _concurrencyService.SetOriginalRowVersion(
            customer,
            rowVersion);

        //-----------------------------------------
        // Domain Update
        //-----------------------------------------

        customer.Update(
            request.Name,
            request.Industry,
            request.Website,
            request.OwnerUserId);

        //-----------------------------------------
        // Save
        //-----------------------------------------

        try
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
        catch (ConcurrencyException)
        {
            throw new ConcurrencyException(
                "The customer was modified by another user. Please refresh and try again.");
        }
    }
}