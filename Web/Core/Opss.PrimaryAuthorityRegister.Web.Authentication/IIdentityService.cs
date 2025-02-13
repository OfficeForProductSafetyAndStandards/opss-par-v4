using Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries;
using Opss.PrimaryAuthorityRegister.Web.Application.Entities;

namespace Opss.PrimaryAuthorityRegister.Web.Authentication;

public interface IIdentityService
{
    Task<HttpObjectResponse<string>> GetJwtToken(GetJwtTokenQuery query);
}
