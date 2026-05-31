using CRM.API.Requests.Customers;
using CRM.API.Responses;
using CRM.API.Responses.Customers;
using CRM.Application.Common.Models;
using CRM.Application.CRM.Customers.Commands.CreateCustomer;
using CRM.Application.CRM.Customers.DTOs;
using CRM.Application.CRM.Customers.Queries.GetCustomers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Controllers.Customers;

[ApiController]
[Route("api/v1/customers")]
[Authorize]
public sealed class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCustomerRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCustomerCommand(
            request.Name,
            request.Industry,
            request.Website,
            request.OwnerUserId);

        var customerId = await _mediator.Send(
            command,
            cancellationToken);

        var response = new CreateCustomerResponseDto
        {
            CustomerId = customerId
        };

        return Ok(
            ApiResponse<CreateCustomerResponseDto>
                .SuccessResponse(
                    response,
                    HttpContext.TraceIdentifier));
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers(
    [FromQuery] GetCustomersQuery query,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(
            ApiResponse<PagedResult<CustomerDto>>
                .SuccessResponse(
                    result,
                    HttpContext.TraceIdentifier));
    }

    [HttpGet("{customerId:guid}")]
    public async Task<IActionResult> GetById(
    Guid customerId,
    CancellationToken cancellationToken)
    {
        var query = new GetCustomerByIdQuery
        {
            CustomerId = customerId
        };

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(
            ApiResponse<CustomerDetailsDto>
                .SuccessResponse(
                    result,
                    HttpContext.TraceIdentifier));
    }
}