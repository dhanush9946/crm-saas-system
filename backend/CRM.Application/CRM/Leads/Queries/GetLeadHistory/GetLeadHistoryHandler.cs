using CRM.Application.Common.Audit;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Leads.DTOs;
using CRM.Domain.Common;
using CRM.Shared.Constants;
using MediatR;

namespace CRM.Application.CRM.Leads.Queries.GetLeadHistory;

public sealed class GetLeadHistoryHandler
    : IRequestHandler<
        GetLeadHistoryQuery,
        PagedResult<LeadHistoryDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ICurrentUser _currentUser;

    public GetLeadHistoryHandler(
        IAuditLogRepository auditLogRepository,
        ICurrentUser currentUser)
    {
        _auditLogRepository = auditLogRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<LeadHistoryDto>> Handle(
        GetLeadHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var filter = new AuditHistoryFilter(
            _currentUser.TenantId,
            EntityTypes.Lead,
            request.LeadId.ToString(),
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

        return new PagedResult<LeadHistoryDto>
        {
            Items = items,
            Page = history.Page,
            PageSize = history.PageSize,
            TotalCount = history.TotalCount
        };
    }

    private static LeadHistoryDto CreateDto(
        AuditLog auditLog)
    {
        var metadata =
            AuditMetadataParser.Parse(auditLog.MetadataJson);

        return new LeadHistoryDto
        {
            Action = auditLog.Action,
            UserId = auditLog.UserId,
            CreatedAtUtc = auditLog.CreatedAtUtc,
            Succeeded = auditLog.Succeeded,
            FailureReason = auditLog.FailureReason,
            Changes = metadata?.Changes?
                .Select(change => new LeadPropertyChangeDto
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
