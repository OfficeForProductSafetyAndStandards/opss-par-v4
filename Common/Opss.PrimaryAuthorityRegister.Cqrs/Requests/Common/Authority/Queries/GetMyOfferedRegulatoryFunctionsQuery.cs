using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Authority.Queries;

[Permission("Authority")]
public class GetMyOfferedRegulatoryFunctionsQuery : IQuery<List<string>>
{ }
