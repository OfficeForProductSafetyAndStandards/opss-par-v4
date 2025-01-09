using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Queries.Dtos;

namespace Opss.PrimaryAuthorityRegister.Common.Requests.Test.Queries;

public class GetTestDataQuery : IQuery<TestDataDto>
{

    public GetTestDataQuery(string value)
    {
        Value = value;
    }

    public string Value { get; set; }
}
