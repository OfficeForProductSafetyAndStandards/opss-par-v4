using Opss.PrimaryAuthorityRegister.Common.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Queries.Dtos;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Queries;

[PermissionFor("Read", "TestData/{Id}", Group = "OPSS")]
public class GetTestDataQuery : IQuery<TestDataDto>
{
    public Guid Id { get; set; }

    public GetTestDataQuery(Guid id)
    {
        Id = id;
    }
}
