using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Queries.Dtos;

namespace Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Queries;

[PermissionFor("Read", "TestData/{Id}")]
public class GetTestDataQuery : IQuery<TestDataDto>
{
    public Guid Id { get; set; }

    public GetTestDataQuery(Guid id)
    {
        Id = id;
    }
}
