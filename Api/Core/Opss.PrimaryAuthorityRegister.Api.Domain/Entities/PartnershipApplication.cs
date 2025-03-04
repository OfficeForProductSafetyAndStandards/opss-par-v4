using Opss.PrimaryAuthorityRegister.Api.Domain.Common;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

public class PartnershipApplication : BaseAuditableEntity
{
    public Authority? Authority { get; private set; }
    public Guid? AuthorityId { get; private set; }
    public string PartnershipType { get; private set; }

    public PartnershipApplication() : base()
    {
        PartnershipType = string.Empty;
    }

    public PartnershipApplication(Guid authorityId, string partnershipType) : base()
    {
        AuthorityId = authorityId;
        PartnershipType = partnershipType;
    }
}
