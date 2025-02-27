using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Services;

public class UserRoleService : IUserRoleService
{
    private readonly IUserIdentityRepository _repository;

    public UserRoleService(IUserIdentityRepository unitOfWork)
    {
        _repository = unitOfWork;
    }

    public AuthenticatedUserIdentity? GetUserWithRolesByEmailAddress(string? email)
    {
        if (email is null) return null;

        var identity = _repository.GetUserIdentiyByEmail(email);

        if (identity is null) return null;

        var roles = identity.Roles.Select(r => new AuthenticatedUserRole(r.Name)).ToArray();
        return new AuthenticatedUserIdentity(identity.Id, identity.EmailAddress, roles);
    }
}