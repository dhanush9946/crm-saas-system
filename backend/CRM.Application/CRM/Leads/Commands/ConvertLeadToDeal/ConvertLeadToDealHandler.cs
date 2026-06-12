using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Customers.Interfaces;
using CRM.Application.CRM.Deals.Interfaces;
using CRM.Application.CRM.Leads.DTOs;
using CRM.Application.CRM.Leads.Interfaces;
using CRM.Domain.CRM.Entities;
using CRM.Domain.CRM.Enums;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.ConvertLeadToDeal;

public sealed class ConvertLeadToDealHandler
    : IRequestHandler<ConvertLeadToDealCommand, LeadConversionResultDto>
{
    private readonly ILeadRepository _leadRepository;
    private readonly IDealRepository _dealRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILeadConversionHistoryRepository _leadConversionHistoryRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public ConvertLeadToDealHandler(
        ILeadRepository leadRepository,
        IDealRepository dealRepository,
        ICustomerRepository customerRepository,
        ILeadConversionHistoryRepository leadConversionHistoryRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _leadRepository = leadRepository;
        _dealRepository = dealRepository;
        _customerRepository = customerRepository;
        _leadConversionHistoryRepository = leadConversionHistoryRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<LeadConversionResultDto> Handle(
        ConvertLeadToDealCommand request,
        CancellationToken cancellationToken)
    {
        // Load lead
        var lead = await _leadRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.LeadId,
            cancellationToken);

        if (lead is null)
        {
            throw new NotFoundException("Lead not found.");
        }

        // Validate current user
        if (_currentUser.UserId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "Current user is not available.");
        }

        // Ensure the lead has already been converted to a customer
        if (!lead.ConvertedCustomerId.HasValue)
        {
            throw new InvalidOperationException(
                "The lead must be converted to a customer before it can be converted to a deal.");
        }

        // Verify the customer still exists
        var customer = await _customerRepository.GetByIdAsync(
            _currentUser.TenantId,
            lead.ConvertedCustomerId.Value,
            cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException("Customer not found.");
        }

        var utcNow = DateTime.UtcNow;

        // Create deal
        var deal = Deal.Create(
            tenantId: _currentUser.TenantId,
            title: request.Title,
            customerId: customer.Id,
            leadId: lead.Id,
            value: request.Value,
            stage: request.Stage,
            expectedCloseDate: request.ExpectedCloseDate,
            ownerUserId: request.OwnerUserId ?? lead.OwnerUserId);

        await _dealRepository.AddAsync(
            deal,
            cancellationToken);

        // Record conversion history
        var history = LeadConversionHistory.Create(
            tenantId: _currentUser.TenantId,
            leadId: lead.Id,
            conversionType: LeadConversionType.Deal,
            relatedEntityId: deal.Id,
            convertedByUserId: _currentUser.UserId,
            convertedAtUtc: utcNow);

        await _leadConversionHistoryRepository.AddAsync(
            history,
            cancellationToken);

        // Commit transaction
        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new LeadConversionResultDto
        {
            LeadId = lead.Id,
            CustomerId = customer.Id,
            DealId = deal.Id,
            ConvertedAtUtc = utcNow,
            LeadStatus = lead.Status
        };
    }
}