using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Authentication.TokenHandler;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.Security.Authentication;

namespace Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;

public class OpenIdConnectTokenService : ITokenService
{
    private readonly OpenIdConnectAuthConfigurations _oidcAuthConfig;
    private readonly IJwtHandler _tokenHandler;
    private readonly IAuthenticatedUserService _authUserService;

    public OpenIdConnectTokenService(
        IOptions<OpenIdConnectAuthConfigurations> oneLoginAuthConfig,
        IJwtHandler tokenHandler,
        IAuthenticatedUserService authUserService)
    {
        ArgumentNullException.ThrowIfNull(oneLoginAuthConfig);

        _oidcAuthConfig = oneLoginAuthConfig.Value;
        _tokenHandler = tokenHandler;
        _authUserService = authUserService;
    }

    public async Task ValidateTokenAsync(string providerKey, string idToken, CancellationToken cancellationToken)
    {
        var response = await _authUserService.GetSigningKeys(providerKey).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpResponseException(response.StatusCode, response.Problem?.Detail);
        }

        var keys = (response.Result?.Keys) ?? throw new AuthenticationException("An empty key set was returned");

        var config = _oidcAuthConfig.Providers[providerKey];

        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = config.IssuerUri.ToString(),
            ValidAudience = config.ClientId,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) => keys,
            ClockSkew = TimeSpan.FromSeconds(config.ClockSkewSeconds)
        };

        _tokenHandler.ValidateToken(idToken, tokenValidationParameters, out SecurityToken _);
    }
}
