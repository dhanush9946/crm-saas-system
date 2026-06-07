using CRM.API.Requests.Activities;
using CRM.API.Responses;
using CRM.API.Responses.Activities;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Activities.Commands.CreateActivity;
using CRM.Application.CRM.Activities.DTOs;
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
}