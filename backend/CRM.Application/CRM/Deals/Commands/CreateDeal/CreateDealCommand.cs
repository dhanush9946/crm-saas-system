using CRM.Domain.CRM.Enums;
using MediatR;

namespace CRM.Application.CRM.Deals.Commands.CreateDeal;

public sealed class CreateDealCommand : IRequest<Guid>
{
    public string Title { get; init; } = string.Empty;

    public Guid CustomerId { get; init; }

    public Guid? LeadId { get; init; }

    public decimal Value { get; init; }

    public DealStage Stage { get; init; }

    public DateOnly? ExpectedCloseDate { get; init; }

    public Guid? OwnerUserId { get; init; }
}
