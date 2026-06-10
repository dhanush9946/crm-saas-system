using CRM.Domain.CRM.Enums;
using MediatR;

namespace CRM.Application.CRM.Deals.Commands.ChangeDealStage;

public sealed class ChangeDealStageCommand : IRequest
{
    public Guid DealId { get; set; }

    public DealStage Stage { get; set; }
}
