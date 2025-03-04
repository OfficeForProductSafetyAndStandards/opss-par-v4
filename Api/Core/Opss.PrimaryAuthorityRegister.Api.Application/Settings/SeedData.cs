namespace Opss.PrimaryAuthorityRegister.Api.Application.Settings;

public class SeedData
{
    private readonly List<SeedIdentity> _identities;
    public List<SeedIdentity> Identities => _identities;

    private readonly List<string> _regulatoryFunctions;
    public List<string> RegulatoryFunctions => _regulatoryFunctions;

    private readonly List<SeedAuthority> _authorities;
    public List<SeedAuthority> Authorities => _authorities;

    public SeedData()
    {
        _identities = [];
        _regulatoryFunctions = [];
        _authorities = [];
    }
}
