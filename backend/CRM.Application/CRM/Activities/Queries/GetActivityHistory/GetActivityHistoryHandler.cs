using CRM.Application.Common.Audit;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Activities.DTOs;
using CRM.Application.Customers.DTOs;
using CRM.Domain.Common;
using CRM.Shared.Constants;
using MediatR;

namespace CRM.Application.CRM.Activities.Queries.GetActivityHistory;

public sealed class GetActivityHistoryHandler
    : IRequestHandler<
        GetActivityHistoryQuery,
        PagedResult<ActivityHistoryDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ICurrentUser _currentUser;

    public GetActivityHistoryHandler(
        IAuditLogRepository auditLogRepository,
        ICurrentUser currentUser)
    {
        _auditLogRepository = auditLogRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<ActivityHistoryDto>> Handle(
        GetActivityHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var filter = new AuditHistoryFilter(
            _currentUser.TenantId,
            EntityTypes.Activity,
            request.ActivityId.ToString(),
            request.Page,
            request.PageSize);

        if (_currentUser.TenantId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "Tenant context is missing.");
        }

        var history =
            await _auditLogRepository.GetEntityHistoryAsync(
                filter,
                cancellationToken);

        var items = history.Items
            .Select(CreateDto)
            .ToList();

        return new PagedResult<ActivityHistoryDto>
        {
            Items = items,
            Page = history.Page,
            PageSize = history.PageSize,
            TotalCount = history.TotalCount
        };
    }

    private static ActivityHistoryDto CreateDto(
        AuditLog auditLog)
    {
        var metadata =
            AuditMetadataParser.Parse(
                auditLog.MetadataJson);

        return new ActivityHistoryDto
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