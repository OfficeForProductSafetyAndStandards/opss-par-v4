using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Queries.Dtos;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Queries;

public class GetTestDataQuery : IQuery<TestDataDto>
{

    public GetTestDataQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
