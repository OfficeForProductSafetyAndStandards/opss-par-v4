using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Common.Constants;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.UnitTests.Entities;

public class PartnershipApplicationTests
{
    [Fact]
    public void WhenConstructing_GivenAuthorityId_PropertiesAreSet()
    {
        var authorityId = Guid.NewGuid();

        var application = new PartnershipApplication(authorityId, PartnershipConstants.PartnershipType.Direct);

        Assert.Equal(authorityId, application.AuthorityId);
        Assert.Equal(PartnershipConstants.PartnershipType.Direct, application.PartnershipType);
        Assert.NotEqual(Guid.Empty, application.Id);
    }
}
