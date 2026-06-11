using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Customers.Interfaces;
using CRM.Application.CRM.Leads.Interfaces;
using CRM.Domain.CRM.Entities;
using CRM.Domain.CRM.Enums;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.ConvertLeadToCustomer;

public sealed class ConvertLeadToCustomerHandler
    : IRequestHandler<ConvertLeadToCustomerCommand, LeadConversionResultDto>
{
    private readonly ILeadRepository _leadRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILeadConversionHistoryRepository _leadConversionHistoryRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ConvertLeadToCustomerHandler(
        ILeadRepository leadRepository,
        ICustomerRepository customerRepository,
        ILeadConversionHistoryRepository leadConversionHistoryRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _leadRepository = leadRepository;
        _customerRepository = customerRepository;
        _leadConversionHistoryRepository = leadConversionHistoryRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<LeadConversionResultDto> Handle(
        ConvertLeadToCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.LeadId,
            cancellationToken);

        if (lead is null)
        {
            throw new NotFoundException("Lead not found.");
        }

        if (_currentUser.UserId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "Current user is not available.");
        }

        var customerName = !string.IsNullOrWhiteSpace(lead.Company)
            ? lead.Company
            : $"{lead.FirstName} {lead.LastName}".Trim();

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new InvalidOperationException(
                "Cannot determine customer name from the lead.");
        }

        var customerExists =
            await _customerRepository.CustomerNameExistsAsync(
                _currentUser.TenantId,
                customerName,
                cancellationToken);

        if (customerExists)
        {
            throw new InvalidOperationException(
                "A customer with the same name already exists.");
        }

        var utcNow = DateTime.UtcNow;

        var customer = Customer.Create(
            tenantId: _currentUser.TenantId,
            name: customerName,
            industry: null,
            website: null,
            ownerUserId: lead.OwnerUserId);

        await _customerRepository.AddAsync(
            customer,
            cancellationToken);

        lead.ConvertToCustomer(
            customer.Id,
            _currentUser.UserId,
            utcNow);

        var history = LeadConversionHistory.Create(
            tenantId: _currentUser.TenantId,
            leadId: lead.Id,
            conversionType: LeadConversionType.Customer,
            relatedEntityId: customer.Id,
            convertedByUserId: _currentUser.UserId,
            convertedAtUtc: utcNow);

        await _leadConversionHistoryRepository.AddAsync(
            history,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new LeadConversionResultDto
        {
            LeadId = lead.Id,
            CustomerId = customer.Id,
            ConvertedAtUtc = utcNow,
            LeadStatus=lead.Status
        };
    }
}