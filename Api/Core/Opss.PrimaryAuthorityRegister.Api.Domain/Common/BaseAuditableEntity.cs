using Opss.PrimaryAuthorityRegister.Api.Domain.Common.Interfaces;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
{
    protected BaseAuditableEntity()
    { }

    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
