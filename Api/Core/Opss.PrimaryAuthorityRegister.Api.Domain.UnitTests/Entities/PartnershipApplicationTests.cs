using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.UnitTests.Entities;

public class PartnershipApplicationTests
{
    [Fact]
    public void WhenConstructing_GivenAuthorityId_PropertiesAreSet()
    {
        var authorityId = Guid.NewGuid();

        var application = new PartnershipApplication(authorityId);

        Assert.Equal(authorityId, application.AuthorityId);
        Assert.NotEqual(Guid.Empty, application.Id);
    }
}
