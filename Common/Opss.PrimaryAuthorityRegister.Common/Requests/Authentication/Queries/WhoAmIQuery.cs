using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;
using Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries.Dtos;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Authentication.Queries;

[MustBeAuthenticated]
public class WhoAmIQuery : IQuery<AuthenticatedUserDetailsDto>
{
    public WhoAmIQuery()
    { }
}
