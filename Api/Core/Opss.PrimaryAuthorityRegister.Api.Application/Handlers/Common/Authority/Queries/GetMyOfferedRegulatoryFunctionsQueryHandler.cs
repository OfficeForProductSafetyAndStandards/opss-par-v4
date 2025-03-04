using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Authority.Queries;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Common.Authority.Queries;

public class GetMyOfferedRegulatoryFunctionsQueryHandler 
    : IRequestHandler<GetMyOfferedRegulatoryFunctionsQuery, List<MyOfferedRegulatoryFunctionDto>>
{
    private readonly IGenericRepository<Domain.Entities.Authority> _authorityRepository;
    private readonly ClaimsPrincipal _claimsPrincipal;

    public GetMyOfferedRegulatoryFunctionsQueryHandler(
        IGenericRepository<Domain.Entities.Authority> authorityRepository,
        ClaimsPrincipal currentUserService)
    {
        _authorityRepository = authorityRepository;
        _claimsPrincipal = currentUserService;
    }
    public async Task<List<MyOfferedRegulatoryFunctionDto>> Handle(
        GetMyOfferedRegulatoryFunctionsQuery request, 
        CancellationToken cancellationToken)
    {
        if (_claimsPrincipal == null) 
            throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized, "You are not authenticated");

        var authorityId = _claimsPrincipal.GetAuthorityId();
        if (authorityId == null)
            throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized, "You are not assigned to an authority");

        var authority = await _authorityRepository.GetByIdAsync(authorityId.Value, a => a.RegulatoryFunctions).ConfigureAwait(false);
        if (authority == null)
            throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized, "Your assigned authority cannot be found");

        var regulatoryFunctions = authority.RegulatoryFunctions.Select(f => 
            new MyOfferedRegulatoryFunctionDto(f.Id, f.Name)).ToList();

        return regulatoryFunctions;
    }
}
