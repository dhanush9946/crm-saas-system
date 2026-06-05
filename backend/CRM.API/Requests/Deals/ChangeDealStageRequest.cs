using CRM.Domain.CRM.Enums;

namespace CRM.API.Requests.Deals;

public sealed class ChangeDealStageRequest
{
    public DealStage Stage { get; set; }
}
