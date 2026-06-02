using MediatR;

public sealed class RestoreCustomerCommand : IRequest
{
    public Guid CustomerId { get; set; }
}