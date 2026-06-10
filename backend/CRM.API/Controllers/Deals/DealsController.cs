using CRM.API.Requests.Deals;
using CRM.API.Responses;
using CRM.API.Responses.Deals;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Deals.Commands.ChangeDealStage;
using CRM.Application.CRM.Deals.Commands.CreateDeal;
using CRM.Application.CRM.Deals.Commands.DeleteDeal;
using CRM.Application.CRM.Deals.Commands.RestoreDeal;
using CRM.Application.CRM.Deals.Commands.UpdateDeal;
using CRM.Application.CRM.Deals.DTOs;
using CRM.Application.CRM.Deals.Queries.GetDealById;
using CRM.Application.CRM.Deals.Queries.GetDealHistory;
using CRM.Application.CRM.Deals.Queries.GetDeals;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.Deals;

[ApiController]
[Route("api/v1/deals")]
[Authorize]
public sealed class DealsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DealsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateDealRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDealCommand
        {
            Title = request.Title,
            CustomerId = request.CustomerId,
            LeadId = request.LeadId,
            Value = request.Value,
            Stage = request.Stage,
            ExpectedCloseDate = request.ExpectedCloseDate,
            OwnerUserId = request.OwnerUserId
        };

        var dealId = await _mediator.Send(
            command,
            cancellationToken);

        var response = new CreateDealResponseDto
        {
            DealId = dealId
        };

        return Ok(
            ApiResponse<CreateDealResponseDto>
                .SuccessResponse(
                    response,
                    HttpContext.TraceIdentifier));
    }

    [HttpGet]
    public async Task<IActionResult> GetDeals(
    [FromQuery] GetDealsQuery query,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(
            ApiResponse<PagedResult<DealListItemDto>>
                .SuccessResponse(
                    result,
                    HttpContext.TraceIdentifier));
    }

    [HttpGet("{dealId:guid}")]
    public async Task<IActionResult> GetById(
    Guid dealId,
    CancellationToken cancellationToken)
    {
        var query = new GetDealByIdQuery
        {
            DealId = dealId
        };

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(
            ApiResponse<DealDetailsDto>
                .SuccessResponse(
                    result,
                    HttpContext.TraceIdentifier));
    }

    [HttpPatch("{dealId:guid}")]
    public async Task<IActionResult> UpdateDeal(
    Guid dealId,
    [FromBody] UpdateDealRequest request,
    CancellationToken cancellationToken)
    {
        var command = new UpdateDealCommand
        {
            DealId = dealId,
            Title = request.Title,
            Value = request.Value,
            ExpectedCloseDate = request.ExpectedCloseDate,
            OwnerUserId = request.OwnerUserId,
            RowVersion = request.RowVersion
        };

        await _mediator.Send(
            command,
            cancellationToken);

        return Ok(
            ApiResponse<string>.SuccessResponse(
                "Deal updated successfully.",
                HttpContext.TraceIdentifier));
    }

    [HttpDelete("{dealId:guid}")]
    public async Task<IActionResult> DeleteDeal(
    Guid dealId,
    CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new DeleteDealCommand
            {
                DealId = dealId
            },
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{dealId:guid}/restore")]
    public async Task<IActionResult> RestoreDeal(
    Guid dealId,
    CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new RestoreDealCommand
            {
                DealId = dealId
            },
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{dealId:guid}/stage")]
    public async Task<IActionResult> ChangeStage(
    Guid dealId,
    [FromBody] ChangeDealStageRequest request,
    CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ChangeDealStageCommand
            {
                DealId = dealId,
                Stage = request.Stage
            },
            cancellationToken);

        return Ok(
            ApiResponse<string>.SuccessResponse(
                "Deal stage updated successfully.",
                HttpContext.TraceIdentifier));
    }

    [HttpGet("{dealId:guid}/history")]
    [ProducesResponseType(typeof(PagedResult<DealHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(
    Guid dealId,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var query = new GetDealHistoryQuery
        {
            DealId = dealId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }
}

