using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Authentication;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authentication;

public class UserClaimsServiceTests
{
    private readonly Mock<IUserIdentityRepository> _userIdentityRepository;
    private readonly UserClaimsService _userClaimsService;

    public UserClaimsServiceTests()
    {
        _userIdentityRepository = new Mock<IUserIdentityRepository>();
        _userClaimsService = new UserClaimsService(_userIdentityRepository.Object);
    }

    [Fact]
    public void GetUserClaims_ShouldReturnEmptyClaims_WhenUserNotFound()
    {
        // Arrange
        string email = "user@example.com";

        // Act
        var claims = _userClaimsService.GetUserClaims(email);

        // Assert
        Assert.NotNull(claims);
        Assert.Empty(claims);
    }

    [Fact]
    public void GetUserClaims_ShouldReturnClaims_WhenUserFound()
    {
        // Arrange
        string email = "user@example.com";
        _userIdentityRepository.Setup(r => r.GetUserIdentiyByEmail(email))
                               .Returns(() => new Domain.Entities.UserIdentity(email));

        // Act
        var claims = _userClaimsService.GetUserClaims(email);

        // Assert
        Assert.NotNull(claims);
        Assert.Contains(claims, c => c.Type == PermissionAttribute.PermissionClaimType);
    }

    [Fact]
    public void GetUserClaims_ShouldReturnAuthorityIdClaim_WhenAuthorityUserFound()
    {
        // Arrange
        string email = "user@example.com";
        var authorityId = Guid.NewGuid();
        var user = new Mock<UserIdentity>(email);
        user.SetupGet(u => u.AuthorityUser).Returns(new AuthorityUser(Guid.NewGuid(), authorityId));

        _userIdentityRepository.Setup(r => r.GetUserIdentiyByEmail(email))
                               .Returns(() => user.Object);

        // Act
        var claims = _userClaimsService.GetUserClaims(email);

        // Assert
        Assert.NotNull(claims);
        Assert.Contains(claims, c => c.Type == Claims.Authority && c.Value == authorityId.ToString());
    }
}