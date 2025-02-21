namespace Opss.PrimaryAuthorityRegister.Authentication.Configuration;

/// <summary>
/// Collection of authentication configurations
/// </summary>
public class OpenIdConnectAuthConfigurations
{
    private readonly Dictionary<string, OpenIdConnectAuthConfiguration> _providers;

    /// <summary>
    /// A collection of auth configs keyd by "ProviderKey"
    /// </summary>
    public Dictionary<string, OpenIdConnectAuthConfiguration> Providers => _providers;

    public OpenIdConnectAuthConfigurations(Dictionary<string, OpenIdConnectAuthConfiguration> providers)
    {
        _providers = providers;
    }

    public OpenIdConnectAuthConfigurations()
    {
        _providers = [];
    }
}
