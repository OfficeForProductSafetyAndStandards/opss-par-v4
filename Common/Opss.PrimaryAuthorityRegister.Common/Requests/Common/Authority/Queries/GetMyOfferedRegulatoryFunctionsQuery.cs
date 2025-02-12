using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Common.Authority.Queries;

[Permission("Authority")]
public class GetMyOfferedRegulatoryFunctionsQuery : IQuery<List<string>>
{ }
