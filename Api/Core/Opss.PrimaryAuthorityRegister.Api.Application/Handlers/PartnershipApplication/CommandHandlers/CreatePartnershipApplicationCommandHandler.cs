using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.PartnershipApplication.Commands;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.PartnershipApplication.CommandHandlers;

public class CreatePartnershipApplicationCommandHandler : IRequestHandler<CreatePartnershipApplicationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ClaimsPrincipal _claimsPrincipal;

    public CreatePartnershipApplicationCommandHandler(IUnitOfWork unitOfWork, ClaimsPrincipal claimsPrincipal)
    {
        _unitOfWork = unitOfWork;
        _claimsPrincipal = claimsPrincipal;
    }

    public Task<Guid> Handle(CreatePartnershipApplicationCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
