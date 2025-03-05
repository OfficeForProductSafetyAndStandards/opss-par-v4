using Opss.PrimaryAuthorityRegister.Api.Domain.Common;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

public class Authority : BaseAuditableEntity
{
    public string Name { get; set; }
    public virtual IReadOnlyCollection<RegulatoryFunction> RegulatoryFunctions => _regulatoryFunctions;
    public virtual IReadOnlyCollection<AuthorityUser> AuthorityUsers => _authorityUsers;
    public virtual IReadOnlyCollection<PartnershipApplication> PartnershipApplications => _partnershipApplications;

    private readonly List<RegulatoryFunction> _regulatoryFunctions;
    private readonly List<AuthorityUser> _authorityUsers;
    private readonly List<PartnershipApplication> _partnershipApplications;

    public Authority(string name) : base()
    {
        Name = name;
        _regulatoryFunctions = new List<RegulatoryFunction>();
        _authorityUsers = new List<AuthorityUser>();
        _partnershipApplications = new List<PartnershipApplication>();
    }

    public Authority(string name, RegulatoryFunction[] regulatoryFunctions) 
        : this(name)
    {
        _regulatoryFunctions.AddRange(regulatoryFunctions);
    }

    public void AddUser(UserIdentity user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var authorityUser = new AuthorityUser(user.Id, this.Id);
        _authorityUsers.Add(authorityUser);
    }

    public void AddRegulatoryFunction(RegulatoryFunction regulatoryFunction)
    {
        ArgumentNullException.ThrowIfNull(regulatoryFunction);

        _regulatoryFunctions.Add(regulatoryFunction);
    }

    public void AddPartnershipApplication(PartnershipApplication application)
    {
        ArgumentNullException.ThrowIfNull(application);

        _partnershipApplications.Add(application);
    }
}
