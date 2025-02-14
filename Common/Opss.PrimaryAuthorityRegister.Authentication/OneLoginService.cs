using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Factories;
using Opss.PrimaryAuthorityRegister.Web.Application.Entities;
using System.Net.Http.Headers;

namespace Opss.PrimaryAuthorityRegister.Authentication;

public class OneLoginService : IOneLoginService
{
    private readonly HttpClient _httpClient;
    private readonly OneLoginAuthConfig _oneLoginAuthConfig;

    public OneLoginService(IOptions<OneLoginAuthConfig> oneLoginAuthConfig, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _oneLoginAuthConfig = oneLoginAuthConfig.Value;
    }

    public async Task<HttpObjectResponse<JsonWebKeySet>> GetSigningKeys()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_oneLoginAuthConfig.Authority + "/.well-known/jwks.json"));

        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        var result = await HttpObjectResponseFactory.DetermineSuccess<JsonWebKeySet>(response).ConfigureAwait(false);

        return result;
    }

    public async Task<HttpObjectResponse<OneLoginUserInfoDto>> GetUserInfo(string accessToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_oneLoginAuthConfig.Authority + "/userinfo"));
 
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        var result = await HttpObjectResponseFactory.DetermineSuccess<OneLoginUserInfoDto>(response).ConfigureAwait(false);

        return result;
    }
}
