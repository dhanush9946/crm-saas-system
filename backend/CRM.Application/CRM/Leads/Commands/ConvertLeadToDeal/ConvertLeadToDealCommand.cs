using CRM.Application.CRM.Leads.DTOs;
using CRM.Domain.CRM.Enums;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.ConvertLeadToDeal;

public sealed class ConvertLeadToDealCommand : IRequest<LeadConversionResultDto>
{
    public Guid LeadId { get; init; }

    public string Title { get; init; } = string.Empty;

    public decimal Value { get; init; }

    public DealStage Stage { get; init; }

    public DateOnly? ExpectedCloseDate { get; init; }

    public Guid? OwnerUserId { get; init; }
}