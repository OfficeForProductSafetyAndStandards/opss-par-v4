using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Services;

public class UserRoleService : IUserRoleService
{
    private readonly IUserIdentityRepository _userIdentityRepository;

    public UserRoleService(IUserIdentityRepository userIdentityRepository)
    {
        _userIdentityRepository = userIdentityRepository;
    }

    public AuthenticatedUserIdentity? GetUserWithRolesByEmailAddress(string? email)
    {
        if (email is null) return null;

        var identity = _userIdentityRepository.GetUserIdentiyByEmail(email);

        if (identity is null) return null;

        var roles = identity.Roles.Select(r => new AuthenticatedUserRole(r.Name)).ToArray();
        return new AuthenticatedUserIdentity(identity.Id, identity.EmailAddress, roles);
    }
}