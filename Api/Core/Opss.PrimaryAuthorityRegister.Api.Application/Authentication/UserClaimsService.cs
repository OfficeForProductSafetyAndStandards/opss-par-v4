using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Authentication;

public class UserClaimsService : IUserClaimsService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserClaimsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IReadOnlyCollection<Claim> GetUserClaims(string email)
    {
        return new List<Claim>
        {
            new Claim(PermissionAttribute.PermissionClaimType, $"Owner/e3e695cc-ca85-43d8-9add-aa004eea5be5", "Create"),
            new Claim(PermissionAttribute.PermissionClaimType, $"Owner/e3e695cc-ca85-43d8-9add-aa004eea5be5", "Write"),
            new Claim(PermissionAttribute.PermissionClaimType, $"Owner/e3e695cc-ca85-43d8-9add-aa004eea5be5", "Read")
        };
    }
}
