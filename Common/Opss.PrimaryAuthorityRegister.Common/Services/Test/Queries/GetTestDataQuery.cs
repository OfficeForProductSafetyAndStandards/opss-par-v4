using Opss.PrimaryAuthorityRegister.Common.Cqrs;
using Opss.PrimaryAuthorityRegister.Common.Services.Test.Queries.Dtos;

namespace Opss.PrimaryAuthorityRegister.Common.Services.Test.Queries;

public class GetTestDataQuery : IQuery<TestDataDto>
{

    public GetTestDataQuery(string value)
    {
        Value = value;
    }

    public string Value { get; set; }
}
