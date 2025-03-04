using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;
using Opss.PrimaryAuthorityRegister.Common.Constants;

namespace Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Authority.Queries;

[MustHaveRole(IdentityConstants.Roles.AuthorityManager, IdentityConstants.Roles.AuthorityMember)]
public class GetMyOfferedRegulatoryFunctionsQuery 
    : IQuery<List<MyOfferedRegulatoryFunctionDto>>
{ }
