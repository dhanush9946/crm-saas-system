using MediatR;

public sealed class RestoreLeadCommand : IRequest
{
    public Guid LeadId { get; set; }
}
