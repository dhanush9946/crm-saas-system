namespace CRM.API.Requests.Customers;

public sealed class UpdateCustomerRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Industry { get; set; }

    public string? Website { get; set; }

    public Guid? OwnerUserId { get; set; }

    public string RowVersion { get; set; } = string.Empty;
}