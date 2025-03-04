using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.PartnershipApplication.Commands;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
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

    public async Task<Guid> Handle(CreatePartnershipApplicationCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (_claimsPrincipal == null)
            throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized, "You are not authenticated");

        var authorityId = _claimsPrincipal.GetAuthorityId();
        if (authorityId == null)
            throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized, "You are not assigned to an authority");

        var authority = await _unitOfWork.Repository<Authority>()
                .GetByIdAsync(authorityId.Value)
                .ConfigureAwait(false);
        if (authority == null)
            throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized, "Your assigned authority cannot be found");

        var application = new Domain.Entities.PartnershipApplication(authority.Id, request.PartnershipType);

        var createdApplication = await 
            _unitOfWork.Repository<Domain.Entities.PartnershipApplication>()
                .AddAsync(application)
                .ConfigureAwait(false);
        
        return createdApplication.Id;        
    }
}
