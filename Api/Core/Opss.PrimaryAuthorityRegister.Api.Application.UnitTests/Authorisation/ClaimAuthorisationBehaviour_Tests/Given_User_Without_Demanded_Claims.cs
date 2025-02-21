using System.Security;
using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Fakes;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;
using Opss.PrimaryAuthorityRegister.Cqrs.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ClaimAuthorisationBehaviour_Tests;

public class Given_User_Without_Demanded_Claims
{
    [Fact]
    public async void When_Invoked_Then_Inner_Behaviour_Is_Not_Called()
    {
        // Arrange
        var innerBehaviourWasCalled = false;
        var command = new MustBeAuthenticatedCommand();
        var claimChecker = new Mock<IClaimChecker>();
        var claimsPrincipal = new FakeClaimsPrincipal("Non-Authenticated");

        claimChecker.Setup(c => c.Demand(claimsPrincipal, command)).Throws(new SecurityException());

        var authoriseBehaviour = new ClaimAuthorisationBehaviour<MustBeAuthenticatedCommand, bool>(claimChecker.Object, claimsPrincipal);

        // Act
        var exception =
                await Record.ExceptionAsync(
                                 () => authoriseBehaviour.Handle(
                                                                        command,
                                                                        () =>
                                                                        {
                                                                            innerBehaviourWasCalled = true;
                                                                            return Task.FromResult(true);
                                                                        },
                                                                        CancellationToken.None));

        // Assert
        Assert.False(innerBehaviourWasCalled);
        Assert.IsType<SecurityException>(exception);
    }

    [Fact]
    public async void When_Invoked_Then_Throws_SecurityException()
    {
        // Arrange
        var command = new MustBeAuthenticatedCommand();
        var claimChecker = new Mock<IClaimChecker>();
        var claimsPrincipal = new FakeClaimsPrincipal("Non-Authenticated");

        claimChecker.Setup(c => c.Demand(claimsPrincipal, command)).Throws(new SecurityException());

        var authoriseBehaviour = new ClaimAuthorisationBehaviour<MustBeAuthenticatedCommand, bool>(claimChecker.Object, claimsPrincipal);

        // Act
        var result =
                await Record.ExceptionAsync(
                                 () => 
                                 authoriseBehaviour.Handle(command, 
                                                           () => Task.FromResult(true),
                                                           CancellationToken.None));

        // Assert
        Assert.NotNull(result);
    }

    [MustBeAuthenticated]
    private class MustBeAuthenticatedCommand : ICommand
    { }
}