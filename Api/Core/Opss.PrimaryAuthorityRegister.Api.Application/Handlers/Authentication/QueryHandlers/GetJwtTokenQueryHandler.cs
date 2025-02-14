using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authentication;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Authentication;
using Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.Net.Http;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Authentication.QueryHandlers;


public class GetJwtTokenQueryHandler : IRequestHandler<GetJwtTokenQuery, string>
{
    private readonly ITokenService _tokenService;
    //private readonly IUsersRepository _usersRepository;
    private readonly IOneLoginService _oneLoginService;
    private readonly IUnitOfWork _unitOfWork;

    public GetJwtTokenQueryHandler(
        ITokenService tokenService,
        //IUsersRepository usersRepository,
        IOneLoginService oneLoginService, 
        IUnitOfWork unitOfWork)
    {
        _tokenService = tokenService;
        //_usersRepository = usersRepository;
        _oneLoginService = oneLoginService;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Handle(GetJwtTokenQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _tokenService.ValidateTokenAsync(request.IdToken, cancellationToken);
        var response = await _oneLoginService.GetUserInfo(request.AccessToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response.StatusCode, response.Problem.Detail);
        }

        var email = response.Result?.Email;

        var token = _tokenService.GenerateJwtToken(email);

        return token;
    }
}