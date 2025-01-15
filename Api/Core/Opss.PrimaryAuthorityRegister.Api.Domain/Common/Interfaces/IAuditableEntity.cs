namespace Opss.PrimaryAuthorityRegister.Api.Domain.Common.Interfaces;

public interface IAuditableEntity : IEntity
{
    DateTime? CreatedDate { get; set; }
    DateTime? UpdatedDate { get; set; }
}
