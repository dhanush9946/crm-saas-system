using CRM.Application.CRM.Leads.DTOs;
using MediatR;

namespace CRM.Application.CRM.Leads.Commands.ConvertLeadToCustomer;

public sealed class ConvertLeadToCustomerCommand
    : IRequest<LeadConversionResultDto>
{
    public Guid LeadId { get; set; }
}