using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.AttributeBasedResourceClaimProvider_Tests;

public class Given_Message_With_AllowAnonymousAttribute
{
    [Fact]
    public void When_Demanding_Claims_Then_InvalidOperationException_Is_Thrown()
    {
        // Arrange
        var attributeBasedResourceClaimProvider =
                new AttributeBasedResourceClaimProvider(Enumerable.Empty<IResourceKeyExpander>());

        // Act
        var exception =
                Record.Exception(
                                 () =>
                                 attributeBasedResourceClaimProvider.GetDemandedClaims(
                                                                                       new CommandWithAllowAnonymousAttribute()));

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    [AllowAnonymous]
    private class CommandWithAllowAnonymousAttribute : ICommand
    {
    }
}