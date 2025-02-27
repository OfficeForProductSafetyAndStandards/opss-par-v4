
namespace Opss.PrimaryAuthorityRegister.Authentication.Entities
{
    public interface IAuthenticatedUserIdentity<TRole> where TRole : IAuthenticatedUserRole
    {
        string EmailAddress { get; }
        Guid Id { get; }
        IReadOnlyCollection<TRole> Roles { get; }
    }
}