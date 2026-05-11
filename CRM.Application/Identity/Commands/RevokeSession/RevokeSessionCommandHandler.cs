using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Identity.Interfaces;
using MediatR;


namespace CRM.Application.Identity.Commands.RevokeSession;

public sealed class RevokeSessionCommandHandler : IRequestHandler<RevokeSessionCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeSessionCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RevokeSessionCommand request,CancellationToken cancellationToken)
    {
        //Load all tokens belonging to the session family
        var tokens = await _refreshTokenRepository.GetByFamilyIdAsync(
                                                request.SessionId,
                                                cancellationToken);

        if (!tokens.Any()) 
        {
            return;
        }

        //multitenant+ ownership validation
        var belongsToCurrentUser = tokens.All(x =>
        x.UserId == _currentUser.UserId &&
        x.TenantId == _currentUser.TenantId);

        if (!belongsToCurrentUser)
        {
            throw new ForbiddenException("You do not have access to this session.");
        }

        //Revoke active tokens
        foreach(var token in tokens)
        {
            if(token.IsActive())
            {
                token.Revoke();
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);


    }
}
