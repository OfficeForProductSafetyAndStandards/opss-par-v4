using Opss.PrimaryAuthorityRegister.Api.Domain.Common;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Entities
{
    public class UserProfile : BaseAuditableEntity
    {
        public UserIdentity? UserIdentity { get; private set; }
        public Guid? UserIdentityId { get; private set; }

        public bool HasAcceptedTermsAndConditions { get; set; }
    }
}