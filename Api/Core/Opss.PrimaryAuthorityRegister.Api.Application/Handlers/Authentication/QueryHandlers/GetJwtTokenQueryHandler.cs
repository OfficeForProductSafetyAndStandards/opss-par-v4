using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Authentication.QueryHandlers;


public class GetJwtTokenQueryHandler : IRequestHandler<GetJwtTokenQuery, string>
{
    //private readonly ITokenService _tokenService;
    //private readonly IUsersRepository _usersRepository;
    //private readonly IOneLoginService _oneLoginService;
    private readonly IUnitOfWork _unitOfWork;

    public GetJwtTokenQueryHandler(
        //ITokenService tokenService,
        //IUsersRepository usersRepository,
        //IOneLoginService oneLoginService, 
        IUnitOfWork unitOfWork)
    {
        //_tokenService = tokenService;
        //_usersRepository = usersRepository;
        //_oneLoginService = oneLoginService;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Handle(GetJwtTokenQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);



        return string.Empty;
    }
}