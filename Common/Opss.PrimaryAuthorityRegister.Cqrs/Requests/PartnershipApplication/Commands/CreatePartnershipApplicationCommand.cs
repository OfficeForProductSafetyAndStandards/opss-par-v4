using Opss.PrimaryAuthorityRegister.Common.Constants;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Cqrs.Requests.PartnershipApplication.Commands;

[MustHaveRole(IdentityConstants.Roles.AuthorityManager, IdentityConstants.Roles.AuthorityMember)]
public class CreatePartnershipApplicationCommand : ICommand<Guid>
{
    public CreatePartnershipApplicationCommand(string partnershipType)
    {
        PartnershipType = partnershipType;
    }

    public string PartnershipType { get; }
}
