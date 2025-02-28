using Opss.PrimaryAuthorityRegister.Authentication.Entities;

namespace Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;

public interface IUserRoleService
{
    public AuthenticatedUserIdentity? GetUserWithRolesByEmailAddress(string? email);
}
