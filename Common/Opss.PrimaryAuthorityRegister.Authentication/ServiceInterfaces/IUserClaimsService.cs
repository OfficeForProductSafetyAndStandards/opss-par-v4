using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;

public interface IUserClaimsService
{
    public IReadOnlyCollection<Claim> GetUserClaims(string email);
}
