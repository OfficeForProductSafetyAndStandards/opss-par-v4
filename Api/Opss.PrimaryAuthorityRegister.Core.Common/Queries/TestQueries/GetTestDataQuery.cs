using Opss.PrimaryAuthorityRegister.Core.Common.Cqrs;
using Opss.PrimaryAuthorityRegister.Core.Common.Dtos.TestDtos;

namespace Opss.PrimaryAuthorityRegister.Core.Common.Queries.TestQueries;

public class GetTestDataQuery : IQuery<TestDataDto>
{

    public GetTestDataQuery(string value)
    {
        Value = value;
    }

    public string Value { get; set; }
}
