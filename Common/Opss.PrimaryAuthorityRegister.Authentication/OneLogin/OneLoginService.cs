using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Factories;
using Opss.PrimaryAuthorityRegister.Http.Services;
using System.Net.Http.Headers;

namespace Opss.PrimaryAuthorityRegister.Authentication.OneLogin;

public class OneLoginService : IAuthenticatedUserService
{
    private readonly IHttpService _httpClient;
    private readonly OpenIdConnectAuthConfig _oneLoginAuthConfig;

    public OneLoginService(IOptions<OpenIdConnectAuthConfig> oneLoginAuthConfig, IHttpService httpClient)
    {
        ArgumentNullException.ThrowIfNull(oneLoginAuthConfig);

        _httpClient = httpClient;
        _oneLoginAuthConfig = oneLoginAuthConfig.Value;
    }

    public async Task<HttpObjectResponse<JsonWebKeySet>> GetSigningKeys()
    {
        var uri = new Uri(_oneLoginAuthConfig.Authority + "/.well-known/jwks.json");
        var result = await _httpClient.HttpSendAsync<JsonWebKeySet>(HttpMethod.Get, uri).ConfigureAwait(false);

        return result;
    }

    public async Task<HttpObjectResponse<AuthenticatedUserInfoDto>> GetUserInfo(string accessToken)
    {
        var uri = new Uri(_oneLoginAuthConfig.Authority + "/userinfo");
        var result = await _httpClient.HttpSendAsync<AuthenticatedUserInfoDto>(HttpMethod.Get, uri, bearerToken: accessToken).ConfigureAwait(false);

        return result;
    }
}
