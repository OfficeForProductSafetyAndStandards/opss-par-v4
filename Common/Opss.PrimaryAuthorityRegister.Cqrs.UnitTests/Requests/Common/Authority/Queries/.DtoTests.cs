using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Authority.Queries;

namespace Opss.PrimaryAuthorityRegister.Cqrs.UnitTests.Requests.Common.Authority.Queries;

public class MyOfferedRegulatoryFunctionDtoTests
{
    [Fact]
    public void Constructor_Sets_Properties()
    {
        var guid = Guid.NewGuid();
        var name = "Name";
        var dto = new MyOfferedRegulatoryFunctionDto(guid, name);

        Assert.Equal(guid, dto.Id);
        Assert.Equal(name, dto.Name);
    }
}

public class MyLocalAuthorityDtoTests
{
    [Fact]
    public void Constructor_Sets_Properties()
    {
        var name = "Name";
        var dto = new MyLocalAuthorityDto(name);

        Assert.Equal(name, dto.Name);
    }
}