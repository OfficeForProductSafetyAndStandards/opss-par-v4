using Opss.PrimaryAuthorityRegister.Common.Constants;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.PartnershipApplication.Commands;
using System.Reflection;

namespace Opss.PrimaryAuthorityRegister.Cqrs.UnitTests.Requests.PartnershipApplication.Commands;

public class CreatePartnershipApplicationCommandTests
{
    [Fact]
    public void GivenCommand_ThenCommandIsDecoratedWithCorrectAuthorizationAttribute()
    {
        // Act
        var attribute = typeof(CreatePartnershipApplicationCommand).GetCustomAttribute<MustHaveRoleAttribute>();

        // Assert
        Assert.NotNull(attribute);
        Assert.Equal(IdentityConstants.Roles.Authority, attribute.Roles);
    }

    [Fact]
    public void GivenConstructor_WhenPartnershipTypeProvided_ThenPropertiesSet()
    {
        var type = PartnershipConstants.PartnershipType.Direct;

        var command = new CreatePartnershipApplicationCommand(type);

        Assert.Equal(type, command.PartnershipType);
    }
}