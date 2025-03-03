namespace Opss.PrimaryAuthorityRegister.Api.Application.Settings;

public class SeedAuthority
{
    public string Name { get; set; }

    private readonly List<string> _identities;
    public List<string> Identities => _identities;

    private readonly List<string> _regulatoryFunctions;
    public List<string> RegulatoryFunctions => _regulatoryFunctions;

    public SeedAuthority()
    {
        _identities = [];
        _regulatoryFunctions = [];
    }
}