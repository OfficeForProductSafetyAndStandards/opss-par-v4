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
        // TODO: Move Requst builder and Derermine Success into the SendAsync function of httpService
        // as Determine Success throws an exception, which needs wrapping into a problem...
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_oneLoginAuthConfig.Authority + "/.well-known/jwks.json"));

        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        var result = await HttpObjectResponseFactory.DetermineSuccess<JsonWebKeySet>(response).ConfigureAwait(false);

        return result;
    }

    public async Task<HttpObjectResponse<AuthenticatedUserInfoDto>> GetUserInfo(string accessToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_oneLoginAuthConfig.Authority + "/userinfo"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        var result = await HttpObjectResponseFactory.DetermineSuccess<AuthenticatedUserInfoDto>(response).ConfigureAwait(false);

        return result;
    }
}
