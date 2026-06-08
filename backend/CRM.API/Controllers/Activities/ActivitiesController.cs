using CRM.API.Requests.Activities;
using CRM.API.Responses;
using CRM.API.Responses.Activities;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Activities.Commands.CreateActivity;
using CRM.Application.CRM.Activities.DTOs;
using CRM.Application.CRM.Activities.Queries.GetActivityById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.Activities;

[ApiController]
[Route("api/v1/activities")]
[Authorize]
public sealed class ActivitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ActivitiesController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateActivityRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateActivityCommand
        {
            Type = request.Type,
            Subject = request.Subject,
            Notes = request.Notes,
            OccurredAtUtc = request.OccurredAtUtc,
            DueAtUtc = request.DueAtUtc,
            RelatedEntityType = request.RelatedEntityType,
            RelatedEntityId = request.RelatedEntityId
        };

        var activityId =
            await _mediator.Send(
                command,
                cancellationToken);

        var response =
            new CreateActivityResponseDto
            {
                ActivityId = activityId
            };

        return Ok(
            ApiResponse<CreateActivityResponseDto>
                .SuccessResponse(
                    response,
                    HttpContext.TraceIdentifier));
    }

    [HttpGet]
    public async Task<IActionResult> GetActivities(
    [FromQuery] GetActivitiesQuery query,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(
            ApiResponse<PagedResult<ActivityDto>>
                .SuccessResponse(
                    result,
                    HttpContext.TraceIdentifier));
    }

    [HttpGet("{activityId:guid}")]
    public async Task<IActionResult> GetById(
    Guid activityId,
    CancellationToken cancellationToken)
    {
        var query = new GetActivityByIdQuery
        {
            ActivityId = activityId
        };

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(
            ApiResponse<ActivityDetailsDto>
                .SuccessResponse(
                    result,
                    HttpContext.TraceIdentifier));
    }

    [HttpPut("{activityId:guid}")]
    public async Task<IActionResult> UpdateActivity(
    Guid activityId,
    [FromBody] UpdateActivityRequestDto request,
    CancellationToken cancellationToken)
    {
        var command = new UpdateActivityCommand
        {
            ActivityId = activityId,
            Type = request.Type,
            Subject = request.Subject,
            Notes = request.Notes,
            OccurredAtUtc = request.OccurredAtUtc,
            DueAtUtc = request.DueAtUtc,
            RowVersion = request.RowVersion
        };

        await _mediator.Send(
            command,
            cancellationToken);

        return Ok(
            ApiResponse<string>.SuccessResponse(
                "Activity updated successfully.",
                HttpContext.TraceIdentifier));
    }
}