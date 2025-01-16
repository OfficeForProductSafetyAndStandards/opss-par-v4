namespace Opss.PrimaryAuthorityRegister.Api.Domain.Common.Interfaces;

public interface IEntity
{
    /// <summary>
    /// When generated, should be created using SequentialGuidValueGenerator 
    /// </summary>
    public Guid Id { get; set; }
}
