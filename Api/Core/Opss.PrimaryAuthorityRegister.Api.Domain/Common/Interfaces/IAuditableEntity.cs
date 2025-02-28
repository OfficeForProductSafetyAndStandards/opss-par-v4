namespace Opss.PrimaryAuthorityRegister.Api.Domain.Common.Interfaces;

public interface IAuditableEntity : IEntity
{
    DateTime? CreatedDate { get; }
    DateTime? UpdatedDate { get; set; }
}
