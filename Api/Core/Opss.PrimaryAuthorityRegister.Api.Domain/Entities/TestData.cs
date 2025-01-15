using Opss.PrimaryAuthorityRegister.Api.Domain.Common;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

public class TestData : BaseAuditableEntity
{
    public TestData(string data) : base()
    {
        Data = data;
    }

    public string Data { get; set; }
}
