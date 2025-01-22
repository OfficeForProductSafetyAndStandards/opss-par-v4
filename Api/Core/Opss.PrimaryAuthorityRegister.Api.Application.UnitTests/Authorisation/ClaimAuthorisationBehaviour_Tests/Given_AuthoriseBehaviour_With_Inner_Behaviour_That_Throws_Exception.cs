using MediatR;
using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Fakes;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ClaimAuthorisationBehaviour_Tests;

public class Given_AuthoriseBehaviour_With_Inner_Behaviour_That_Throws_Exception
{
    [Fact]
    public async void When_Invoked_Bubbles_Exception()
    {
        // Arrange
        var mockNextDelegate = new Mock<RequestHandlerDelegate<string>>();

        mockNextDelegate
            .Setup(next => next())
            .Throws< Exception >(() => throw new Exception() );

        var behaviour = new ClaimAuthorisationBehaviour<IRequest, string>(
            new Mock<IClaimChecker>().Object, new FakeClaimsPrincipal("A User"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            behaviour.Handle(new Mock<IRequest>().Object, 
                             mockNextDelegate.Object, 
                             CancellationToken.None));
    }
}