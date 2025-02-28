using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Authentication;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authentication;

public class UserClaimsServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UserClaimsService _userClaimsService;

    public UserClaimsServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userClaimsService = new UserClaimsService(_unitOfWorkMock.Object);
    }

    [Fact]
    public void GetUserClaims_ShouldReturnExpectedClaims_WhenEmailIsProvided()
    {
        // Arrange
        string email = "user@example.com";

        // Act
        var claims = _userClaimsService.GetUserClaims(email);

        // Assert
        Assert.NotNull(claims);
        Assert.Contains(claims, c => c.Type == PermissionAttribute.PermissionClaimType);
    }
}