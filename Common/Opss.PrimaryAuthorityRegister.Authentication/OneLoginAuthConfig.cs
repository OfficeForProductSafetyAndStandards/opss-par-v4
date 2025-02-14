namespace Opss.PrimaryAuthorityRegister.Web.Application.Entities;

/// <summary>
/// Authentication configuration
/// </summary>
public class OneLoginAuthConfig
{
    /// <summary>
    /// Max-age of cookie, in minutes
    /// </summary>
    public int CookieMaxAge { get; set; }

    /// <summary>
    /// OpenID Connect Authority for GOV.UK OneLogin
    /// </summary>
    public string Authority { get; set; }

    /// <summary>
    /// OpenID Connect Client Id for GOV.UK OneLogin
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// RSA Private Key
    /// </summary>
    public string RsaPrivateKey { get; set; }

    /// <summary>
    /// Post logout redirect URI
    /// </summary>
    public string PostLogoutRedirectUri { get; set; }


    public short ClockSkewSeconds { get; set; }
}
