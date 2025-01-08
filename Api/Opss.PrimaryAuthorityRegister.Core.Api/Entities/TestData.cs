using Opss.PrimaryAuthorityRegister.Core.Common.Dtos.TestDtos;

namespace Opss.PrimaryAuthorityRegister.Core.Api.Entities;

public class TestData
{
    public TestData(TestDataDto data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Id = data.Id;
    }

    public Guid Id { get; set; }
}
