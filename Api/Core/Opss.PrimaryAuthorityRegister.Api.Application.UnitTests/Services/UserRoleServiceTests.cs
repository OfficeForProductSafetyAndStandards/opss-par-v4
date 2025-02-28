using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Application.Services;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Services;

public class UserRoleServiceTests
{
    private readonly Mock<IUserIdentityRepository> _repositoryMock;
    private readonly UserRoleService _userRoleService;

    public UserRoleServiceTests()
    {
        _repositoryMock = new Mock<IUserIdentityRepository>();
        _userRoleService = new UserRoleService(_repositoryMock.Object);
    }

    [Fact]
    public void GetUserWithRolesByEmailAddress_ShouldReturnNull_WhenEmailIsNull()
    {
        // Act
        var result = _userRoleService.GetUserWithRolesByEmailAddress(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUserWithRolesByEmailAddress_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        var email = "test@example.com";
        _repositoryMock.Setup(repo => repo.GetUserIdentiyByEmail(email)).Returns((UserIdentity?)null);

        // Act
        var result = _userRoleService.GetUserWithRolesByEmailAddress(email);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetUserWithRolesByEmailAddress_ShouldReturnAuthenticatedUser_WhenUserExists()
    {
        // Arrange
        var email = "test@example.com";
        var roles = new[] { new Role("Admin"), new Role("User") };
        var user = new UserIdentity(email, roles);

        _repositoryMock.Setup(repo => repo.GetUserIdentiyByEmail(email)).Returns(user);

        // Act
        var result = _userRoleService.GetUserWithRolesByEmailAddress(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.EmailAddress);
        Assert.Equal(roles.Select(r => r.Name), result.Roles.Select(r => r.Name));
    }
}
