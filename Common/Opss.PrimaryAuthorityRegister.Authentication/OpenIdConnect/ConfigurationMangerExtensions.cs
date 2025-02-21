using Microsoft.Extensions.Configuration;

namespace Opss.PrimaryAuthorityRegister.Authentication.OpenIdConnect;

public static class ConfigurationMangerExtensions
{
    public static bool TryGetSection<T>(this ConfigurationManager manager, string sectionName, out T? typedSection) where T : class
    {
        ArgumentNullException.ThrowIfNull(manager);

        var section = manager.GetSection(sectionName);
        typedSection = section.Get<T>();

        return typedSection != null;
    }
}