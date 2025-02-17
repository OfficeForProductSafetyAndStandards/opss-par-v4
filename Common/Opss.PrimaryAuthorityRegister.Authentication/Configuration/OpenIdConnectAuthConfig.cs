namespace Opss.PrimaryAuthorityRegister.Authentication.Configuration;

/// <summary>
/// Authentication configuration
/// </summary>
public class OpenIdConnectAuthConfig
{
    /// <summary>
    /// Max-age of cookie, in minutes
    /// </summary>
    public int CookieMaxAge { get; set; }

    /// <summary>
    /// OpenID Connect Authority for GOV.UK OneLogin
    /// </summary>
    public required string Authority { get; set; }

    /// <summary>
    /// OpenID Connect Client Id for GOV.UK OneLogin
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// RSA Private Key
    /// </summary>
    public required string RsaPrivateKey { get; set; }

    /// <summary>
    /// Post logout redirect URI
    /// </summary>
    public required Uri PostLogoutRedirectUri { get; set; }


    public short ClockSkewSeconds { get; set; }
}
