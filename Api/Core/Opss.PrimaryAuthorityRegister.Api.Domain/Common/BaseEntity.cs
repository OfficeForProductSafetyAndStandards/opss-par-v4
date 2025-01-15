using Opss.PrimaryAuthorityRegister.Api.Domain.Common.Interfaces;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Common;

public abstract class BaseEntity : IEntity
{
    protected BaseEntity()
    {
        Id = Ulid.NewUlid().ToGuid();
    }

    public Guid Id { get; set; }
}
