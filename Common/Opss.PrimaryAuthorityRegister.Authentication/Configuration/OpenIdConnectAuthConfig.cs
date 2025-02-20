using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace Opss.PrimaryAuthorityRegister.Authentication.Configuration;

public class OpenIdConnectAuthConfigurations
{
    private readonly Dictionary<string, OpenIdConnectAuthConfig> _providers;

    public Dictionary<string, OpenIdConnectAuthConfig> Providers => _providers;

    public void AddConfig(string key, OpenIdConnectAuthConfig config)
    {
        _providers.Add(key, config);
    }

    public OpenIdConnectAuthConfigurations(Dictionary<string, OpenIdConnectAuthConfig> providers)
    {
        _providers = providers;
    }
    public OpenIdConnectAuthConfigurations()
    {
        _providers = [];
    }
}

/// <summary>
/// Authentication configuration
/// </summary>
public class OpenIdConnectAuthConfig
{
    public required string ProviderKey { get; set; }
    /// <summary>
    /// Max-age of cookie, in minutes
    /// </summary>
    public int CookieMaxAge { get; set; }

    /// <summary>
    /// OpenID Connect Authority for GOV.UK OneLogin
    /// </summary>
    public required Uri AuthorityUri { get; set; }
    public required Uri IssuerUri { get; set; }

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
    public required string WellKnownPath { get; set; }
    public required string UserInfoPath { get; set; }
    public required string AccessTokenPath { get; set; }
    public required string CallbackPath { get; set; }

    public string? ClientSecret { get; set; }
}
