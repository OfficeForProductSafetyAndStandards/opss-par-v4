using MediatR;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Authentication.QueryHandlers;


public class GetJwtTokenQueryHandler : IRequestHandler<GetJwtTokenQuery, string>
{
    private readonly ITokenService _tokenService;
    private readonly IAuthenticatedUserService _oneLoginService;

    public GetJwtTokenQueryHandler(
        ITokenService tokenService,
        IAuthenticatedUserService oneLoginService)
    {
        _tokenService = tokenService;
        _oneLoginService = oneLoginService;
    }

    public async Task<string> Handle(GetJwtTokenQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _tokenService.ValidateTokenAsync(request.IdToken, cancellationToken).ConfigureAwait(false);
        var response = await _oneLoginService.GetUserInfo(request.AccessToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response.StatusCode, response.Problem.Detail);
        }

        var email = response.Result?.Email;

        var token = _tokenService.GenerateJwtToken(email);

        return token;
    }
}