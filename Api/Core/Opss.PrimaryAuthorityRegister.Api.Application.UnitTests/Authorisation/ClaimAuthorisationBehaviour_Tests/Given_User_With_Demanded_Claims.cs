using MediatR;
using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Authorisation;
using Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Fakes;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ClaimAuthorisationBehaviour_Tests;

public class Given_User_With_Demanded_Claims
{
    [Fact]
    public async void When_Invoked_Then_Invokes_Inner_Behaviour()
    {
        // Arrange
        var mockNextDelegate = new Mock<RequestHandlerDelegate<string>>();

        mockNextDelegate
            .Setup(next => next())
            .ReturnsAsync("ExpectedResponse");

        var behaviour = new ClaimAuthorisationBehaviour<IRequestBase, string>(
            new Mock<IClaimChecker>().Object, new FakeClaimsPrincipal("A User"));

        // Act
        var result = await behaviour.Handle(new Mock<IRequestBase>().Object, 
                                            mockNextDelegate.Object, 
                                            CancellationToken.None);

        // Assert
        Assert.Equal("ExpectedResponse", result);
    }
}