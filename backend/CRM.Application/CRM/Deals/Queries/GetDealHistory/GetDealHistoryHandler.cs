using CRM.Application.Common.Audit;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Deals.DTOs;
using CRM.Domain.Common;
using CRM.Shared.Constants;
using MediatR;

namespace CRM.Application.CRM.Deals.Queries.GetDealHistory;

public sealed class GetDealHistoryHandler
    : IRequestHandler<
        GetDealHistoryQuery,
        PagedResult<DealHistoryDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ICurrentUser _currentUser;

    public GetDealHistoryHandler(
        IAuditLogRepository auditLogRepository,
        ICurrentUser currentUser)
    {
        _auditLogRepository = auditLogRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<DealHistoryDto>> Handle(
        GetDealHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var filter = new AuditHistoryFilter(
            _currentUser.TenantId,
            EntityTypes.Deal,
            request.DealId.ToString(),
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

        return new PagedResult<DealHistoryDto>
        {
            Items = items,
            Page = history.Page,
            PageSize = history.PageSize,
            TotalCount = history.TotalCount
        };
    }

    private static DealHistoryDto CreateDto(
        AuditLog auditLog)
    {
        var metadata =
            AuditMetadataParser.Parse(auditLog.MetadataJson);

        return new DealHistoryDto
        {
            Action = auditLog.Action,
            UserId = auditLog.UserId,
            ChangedByUserId = auditLog.UserId,
            CreatedAtUtc = auditLog.CreatedAtUtc,
            Timestamp = auditLog.CreatedAtUtc,
            EntityType = auditLog.EntityType,
            EntityId = auditLog.EntityId,
            Succeeded = auditLog.Succeeded,
            FailureReason = auditLog.FailureReason,
            ChangesJson = auditLog.MetadataJson,
            Changes = metadata?.Changes?
                .Select(change => new DealPropertyChangeDto
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
