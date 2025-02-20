using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries;

[MustBeAuthenticated]
public class GetJwtQuery : IQuery<string>
{
    /// <summary>
    /// GOV.UK One Login ID token
    /// </summary>
    public string IdToken { get; private set; }

    /// <summary>
    /// GOV.UK One Login access token
    /// </summary>
    public string AccessToken { get; private set; }

    /// <summary>
    /// GOV.UK One Login email address
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// The Authentication provider
    /// </summary>
    public string ProviderKey { get; set; }

    public GetJwtQuery(string providerKey, string idToken, string accessToken, string? email = null)
    {
        ProviderKey = providerKey;
        IdToken = idToken;
        AccessToken = accessToken;
        Email = email;
    }
}