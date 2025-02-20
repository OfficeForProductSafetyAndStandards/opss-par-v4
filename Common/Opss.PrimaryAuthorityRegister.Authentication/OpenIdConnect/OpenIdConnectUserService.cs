using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Configuration;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Common.ExtensionMethods;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Services;

namespace Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;

public class OpenIdConnectUserService : IAuthenticatedUserService
{
    private readonly IHttpService _httpClient;
    private readonly OpenIdConnectAuthConfigurations _oidcAuthConfig;

    public OpenIdConnectUserService(IOptions<OpenIdConnectAuthConfigurations> oneLoginAuthConfig, IHttpService httpClient)
    {
        ArgumentNullException.ThrowIfNull(oneLoginAuthConfig);

        _httpClient = httpClient;
        _oidcAuthConfig = oneLoginAuthConfig.Value;
    }

    public async Task<HttpObjectResponse<JsonWebKeySet>> GetSigningKeys(string providerKey)
    {
        var uri = _oidcAuthConfig.Providers[providerKey].AuthorityUri.AppendPath("/.well-known/jwks.json");
        var result = await _httpClient.HttpSendAsync<JsonWebKeySet>(HttpMethod.Get, uri).ConfigureAwait(false);

        return result;
    }

    public async Task<HttpObjectResponse<AuthenticatedUserInfoDto>> GetUserInfo(string providerKey, string accessToken)
    {
        var uri = _oidcAuthConfig.Providers[providerKey].AuthorityUri.AppendPath(_oidcAuthConfig.Providers[providerKey].UserInfoPath);
        var result = await _httpClient.HttpSendAsync<AuthenticatedUserInfoDto>(HttpMethod.Get, uri, bearerToken: accessToken).ConfigureAwait(false);

        return result;
    }
}
