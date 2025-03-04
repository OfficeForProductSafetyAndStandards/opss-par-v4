using Opss.PrimaryAuthorityRegister.Api.Domain.Common;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

public class RegulatoryFunction : BaseAuditableEntity
{
    public string Name { get; set; }
    public IReadOnlyCollection<Authority> Authorities => _authorities;

    private readonly List<Authority> _authorities;

    public RegulatoryFunction(string name) : base()
    {
        Name = name;
        _authorities = new List<Authority>();
    }
}