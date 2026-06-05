using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.CRM.Deals.DTOs;
using CRM.Application.CRM.Deals.Interfaces;
using MediatR;

namespace CRM.Application.CRM.Deals.Queries.GetDealById;

public sealed class GetDealByIdHandler
    : IRequestHandler<GetDealByIdQuery, DealDetailsDto>
{
    private readonly IDealRepository _dealRepository;
    private readonly ICurrentUser _currentUser;

    public GetDealByIdHandler(
        IDealRepository dealRepository,
        ICurrentUser currentUser)
    {
        _dealRepository = dealRepository;
        _currentUser = currentUser;
    }

    public async Task<DealDetailsDto> Handle(
        GetDealByIdQuery request,
        CancellationToken cancellationToken)
    {
        var deal = await _dealRepository.GetByIdAsync(
            _currentUser.TenantId,
            request.DealId,
            cancellationToken);

        if (deal is null)
        {
            throw new NotFoundException(
                $"Deal '{request.DealId}' was not found.");
        }

        return new DealDetailsDto
        {
            Id = deal.Id,
            Title = deal.Title,
            CustomerId = deal.CustomerId,
            LeadId = deal.LeadId,
            Value = deal.Value,
            Probability = deal.Probability,
            Stage = deal.Stage.ToString(),
            ExpectedCloseDate = deal.ExpectedCloseDate,
            OwnerUserId = deal.OwnerUserId,
            CreatedAtUtc = deal.CreatedAtUtc,
            UpdatedAtUtc = deal.UpdatedAtUtc,
            RowVersion = Convert.ToBase64String(
                deal.RowVersion!)
        };
    }
}
