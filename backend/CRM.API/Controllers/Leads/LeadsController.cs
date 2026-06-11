using CRM.API.Requests.Leads;
using CRM.API.Responses;
using CRM.API.Responses.Leads;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Leads.Commands.AssignLead;
using CRM.Application.CRM.Leads.Commands.ChangeLeadStatus;
using CRM.Application.CRM.Leads.Commands.ConvertLeadToCustomer;
using CRM.Application.CRM.Leads.Commands.CreateLead;
using CRM.Application.CRM.Leads.Commands.DeleteLead;
using CRM.Application.CRM.Leads.Commands.UpdateLead;
using CRM.Application.CRM.Leads.DTOs;
using CRM.Application.CRM.Leads.Queries.GetLeadHistory;
using CRM.Application.CRM.Leads.Queries.GetLeads;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.Leads;

[ApiController]
[Route("api/v1/leads")]
[Authorize]
public sealed class LeadsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeadsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateLeadRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLeadCommand
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Company = request.Company,
            Source = request.Source,
            OwnerUserId = request.OwnerUserId
        };

        var leadId = await _mediator.Send(
            command,
            cancellationToken);

        var response = new CreateLeadResponseDto
        {
            LeadId = leadId
        };

        return Ok(
            ApiResponse<CreateLeadResponseDto>
                .SuccessResponse(
                    response,
                    HttpContext.TraceIdentifier));
    }

    [HttpGet]
    public async Task<IActionResult> GetLeads(
    [FromQuery] GetLeadsQuery query,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(
            ApiResponse<PagedResult<LeadDto>>
                .SuccessResponse(
                    result,
                    HttpContext.TraceIdentifier));
    }

    [HttpGet("{leadId:guid}")]
    public async Task<IActionResult> GetById(
    Guid leadId,
    CancellationToken cancellationToken)
    {
        var query = new GetLeadByIdQuery
        {
            LeadId = leadId
        };

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(
            ApiResponse<LeadDetailsDto>
                .SuccessResponse(
                    result,
                    HttpContext.TraceIdentifier));
    }

    [HttpPut("{leadId:guid}")]
    public async Task<IActionResult> UpdateLead(
    Guid leadId,
    [FromBody] UpdateLeadRequest request,
    CancellationToken cancellationToken)
    {
        var command = new UpdateLeadCommand
        {
            LeadId = leadId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Company = request.Company,
            Source = request.Source,
            OwnerUserId = request.OwnerUserId,
            RowVersion = request.RowVersion
        };

        await _mediator.Send(
            command,
            cancellationToken);

        return Ok(
            ApiResponse<string>.SuccessResponse(
                "Lead updated successfully.",
                HttpContext.TraceIdentifier));
    }

    [HttpDelete("{leadId:guid}")]
    public async Task<IActionResult> DeleteLead(
    Guid leadId,
    CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new DeleteLeadCommand
            {
                LeadId = leadId
            },
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{leadId:guid}/restore")]
    public async Task<IActionResult> RestoreLead(
    Guid leadId,
    CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new RestoreLeadCommand
            {
                LeadId = leadId
            },
            cancellationToken);

        return NoContent();
    }

    [HttpGet("{leadId:guid}/history")]
    [ProducesResponseType(typeof(PagedResult<LeadHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(
    Guid leadId,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var query = new GetLeadHistoryQuery
        {
            LeadId = leadId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{leadId:guid}/convert")]
    public async Task<IActionResult> ConvertToCustomer(
    Guid leadId,
    CancellationToken cancellationToken)
    {
        var command = new ConvertLeadToCustomerCommand
        {
            LeadId = leadId
        };

        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(
            ApiResponse<LeadConversionResultDto>
                .SuccessResponse(
                    result,
                    HttpContext.TraceIdentifier));
    }


    [HttpPatch("{leadId:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
    Guid leadId,
    [FromBody] ChangeLeadStatusRequest request,
    CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ChangeLeadStatusCommand
            {
                LeadId = leadId,
                Status = request.Status
            },
            cancellationToken);

        return Ok(
            ApiResponse<string>.SuccessResponse(
                "Lead status updated successfully.",
                HttpContext.TraceIdentifier));
    }

    [HttpPatch("{leadId:guid}/assign")]
    public async Task<IActionResult> AssignLead(
    Guid leadId,
    [FromBody] AssignLeadRequest request,
    CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new AssignLeadCommand
            {
                LeadId = leadId,
                OwnerUserId = request.OwnerUserId
            },
            cancellationToken);

        return Ok(
            ApiResponse<string>.SuccessResponse(
                "Lead assigned successfully.",
                HttpContext.TraceIdentifier));
    }
}
