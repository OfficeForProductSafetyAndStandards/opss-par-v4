using MediatR;
using Opss.PrimaryAuthorityRegister.Authentication.Jwt;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Authentication.Queries;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Authentication.QueryHandlers;


public class GetJwtQueryHandler : IRequestHandler<GetJwtQuery, string>
{
    private readonly ITokenService _tokenService;
    private readonly IJwtService _jwtService;
    private readonly IAuthenticatedUserService _oneLoginService;

    public GetJwtQueryHandler(
        ITokenService tokenService,
        IAuthenticatedUserService oneLoginService,
        IJwtService jwtService)
    {
        _tokenService = tokenService;
        _oneLoginService = oneLoginService;
        _jwtService = jwtService;
    }

    public async Task<string> Handle(GetJwtQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _tokenService.ValidateTokenAsync(request.ProviderKey, request.IdToken, cancellationToken).ConfigureAwait(false);
        var response = await _oneLoginService.GetUserInfo(request.ProviderKey, request.AccessToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response.StatusCode, response.Problem?.Detail);
        }

        var email = response.Result?.Email;

        var token = _jwtService.GenerateJwt(email);

        return token;
    }
}