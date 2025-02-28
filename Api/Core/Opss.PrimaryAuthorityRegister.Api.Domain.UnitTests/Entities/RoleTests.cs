using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.UnitTests.Entities;

public class RoleTests
{
    [Fact]
    public void Constructor_ShouldInitializeName()
    {
        // Arrange
        var roleName = "Admin";

        // Act
        var role = new Role(roleName);

        // Assert
        Assert.Equal(roleName, role.Name);
    }

    [Fact]
    public void Constructor_ShouldInitializeEmptyUserIdentitiesList()
    {
        // Arrange
        var roleName = "User";

        // Act
        var role = new Role(roleName);

        // Assert
        Assert.NotNull(role.UserIdentities);
        Assert.Empty(role.UserIdentities);
    }
}
