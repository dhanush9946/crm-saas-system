namespace CRM.API.Requests.Deals;

public sealed class UpdateDealRequest
{
    public string Title { get; set; } = string.Empty;

    public decimal Value { get; set; }

    public DateOnly? ExpectedCloseDate { get; set; }

    public Guid? OwnerUserId { get; set; }

    public string RowVersion { get; set; } = string.Empty;
}
