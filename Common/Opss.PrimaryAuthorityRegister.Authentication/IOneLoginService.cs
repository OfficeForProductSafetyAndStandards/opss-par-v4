using MediatR;
using Microsoft.IdentityModel.Tokens;
using Opss.PrimaryAuthorityRegister.Common;
using Opss.PrimaryAuthorityRegister.Http.Entities;
using Opss.PrimaryAuthorityRegister.Http.Services;

namespace Opss.PrimaryAuthorityRegister.Authentication;

public interface IOneLoginService
{
    Task<HttpObjectResponse<JsonWebKeySet>> GetSigningKeys();
    Task<HttpObjectResponse<OneLoginUserInfoDto>> GetUserInfo(string accessToken);
}
