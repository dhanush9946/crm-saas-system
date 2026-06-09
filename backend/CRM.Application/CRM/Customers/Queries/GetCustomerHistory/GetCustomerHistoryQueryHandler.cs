using CRM.Application.Common.Audit;
using CRM.Application.Common.Interfaces;
using CRM.Shared.Constants;
using CRM.Application.Common.Models;
using CRM.Application.Customers.DTOs;
using CRM.Domain.Common;
using MediatR;

namespace CRM.Application.Customers.Queries.GetCustomerHistory;

public sealed class GetCustomerHistoryQueryHandler
    : IRequestHandler<
        GetCustomerHistoryQuery,
        PagedResult<CustomerHistoryDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ICurrentUser _currentUser;

    public GetCustomerHistoryQueryHandler(
        IAuditLogRepository auditLogRepository,
        ICurrentUser currentUser)
    {
        _auditLogRepository = auditLogRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<CustomerHistoryDto>> Handle(
        GetCustomerHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var filter = new AuditHistoryFilter(
            _currentUser.TenantId,
            EntityTypes.Customer,
            request.CustomerId.ToString(),
            request.Page,
            request.PageSize);

        if (_currentUser.TenantId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "Tenant context is missing.");
        }

        var history = await _auditLogRepository.GetEntityHistoryAsync(
            filter,
            cancellationToken);

        var items = history.Items
            .Select(CreateDto)
            .ToList();

        return new PagedResult<CustomerHistoryDto>
        {
            Items = items,
            Page = history.Page,
            PageSize = history.PageSize,
            TotalCount = history.TotalCount
        };
    }

    private static CustomerHistoryDto CreateDto(
        AuditLog auditLog)
    {
        var metadata =
            AuditMetadataParser.Parse(auditLog.MetadataJson);

        return new CustomerHistoryDto
        {
            Action = auditLog.Action,
            UserId = auditLog.UserId,
            CreatedAtUtc = auditLog.CreatedAtUtc,
            Succeeded = auditLog.Succeeded,
            FailureReason = auditLog.FailureReason,
            Changes = metadata?.Changes?
                .Select(change => new PropertyChangeDto
                {
                    PropertyName = change.PropertyName,
                    OldValue = change.OldValue,
                    NewValue = change.NewValue
                })
                .ToList()
                ?? []
        };
    }
}