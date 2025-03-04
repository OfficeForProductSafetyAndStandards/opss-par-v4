using Opss.PrimaryAuthorityRegister.Api.Domain.Common;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

public class PartnershipApplication : BaseAuditableEntity
{
    public Authority? Authority { get; private set; }
    public Guid? AuthorityId { get; private set; }

    public PartnershipApplication() : base()
    { }

    public PartnershipApplication(Guid authorityId) : base()
    {
        AuthorityId = authorityId;
    }
}
