using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries;
using Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries.Dtos;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Authentication.QueryHandlers;

public class WhoAmIQueryHandler : IRequestHandler<WhoAmIQuery, AuthenticatedUserDetailsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public WhoAmIQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticatedUserDetailsDto> Handle(WhoAmIQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

                

        return new AuthenticatedUserDetailsDto();
    }
}