using System.Collections.ObjectModel;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Settings;

public class SeedData
{
    private readonly List<SeedIdentity> _identities;

    public List<SeedIdentity> Identities => _identities;

    public SeedData(ReadOnlyCollection<SeedIdentity> identities)
    {
        _identities = identities.ToList();
    }

    public SeedData()
    {
        _identities = [];
    }
}
