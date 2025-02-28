namespace Opss.PrimaryAuthorityRegister.Authentication.Configuration;

/// <summary>
/// Authentication configuration
/// </summary>
public class OpenIdConnectAuthConfiguration
{
    /// <summary>
    /// The OpenIdConnectAuthConfigurations key for accessing this 
    /// entity in the collection
    /// </summary>
    public required string ProviderKey { get; set; }
    /// <summary>
    /// Max-age of cookie, in minutes
    /// </summary>
    public int CookieMaxAge { get; set; }

    /// <summary>
    /// OpenID Connect Authority
    /// This is the base Uri of the authentication provider
    /// </summary>
    public required Uri AuthorityUri { get; set; }

    /// <summary>
    /// OpenID Connect Issuer (Used in Token Validation)
    /// </summary>
    public required Uri IssuerUri { get; set; }

    /// <summary>
    /// OpenID Connect Client Id
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// RSA Private Key
    /// </summary>
    public string? RsaPrivateKey { get; set; }

    /// <summary>
    /// Open Id connect client secret
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Post logout redirect URI
    /// </summary>
    public required Uri PostLogoutRedirectUri { get; set; }

    /// <summary>
    /// The tollerance to apply when validating tokens
    /// </summary>
    public short ClockSkewSeconds { get; set; }

    /// <summary>
    /// Path to the well-known openid configuration endpoint
    /// </summary>
    public required string WellKnownPath { get; set; }

    /// <summary>
    /// Open Id connect UserInfo endpoint
    /// </summary>
    public required string UserInfoPath { get; set; }

    /// <summary>
    /// Jwt security token audience access token endpoint
    /// </summary>
    public required string AccessTokenPath { get; set; }

    /// <summary>
    /// Open Id connect callback path
    /// </summary>
    public required string CallbackPath { get; set; }
}
