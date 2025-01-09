using Opss.PrimaryAuthorityRegister.Api.Common.Cqrs;
using Opss.PrimaryAuthorityRegister.Api.Common.Services.Test.Queries.Dtos;

namespace Opss.PrimaryAuthorityRegister.Api.Common.Services.Test.Queries;

public class GetTestDataQuery : IQuery<TestDataDto>
{

    public GetTestDataQuery(string value)
    {
        Value = value;
    }

    public string Value { get; set; }
}
