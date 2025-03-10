using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Profile.Queries
{
    [MustBeAuthenticated]
    public class GetMyProfileQuery : IQuery<MyProfileDto>
    {

    }
}
