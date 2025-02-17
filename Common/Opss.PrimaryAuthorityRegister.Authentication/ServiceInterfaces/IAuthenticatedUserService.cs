using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Authentication.Entities;
using Opss.PrimaryAuthorityRegister.Http.Entities;

namespace Opss.PrimaryAuthorityRegister.Authentication.ServiceInterfaces;

public interface IAuthenticatedUserService
{
    Task<HttpObjectResponse<JsonWebKeySet>> GetSigningKeys();
    Task<HttpObjectResponse<AuthenticatedUserInfoDto>> GetUserInfo(string accessToken);
}
