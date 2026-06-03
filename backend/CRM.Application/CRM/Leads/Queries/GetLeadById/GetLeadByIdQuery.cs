using CRM.Application.CRM.Leads.DTOs;
using MediatR;

public sealed class GetLeadByIdQuery
    : IRequest<LeadDetailsDto>
{
    public Guid LeadId { get; set; }
}
