using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.UnitTests.Entities;

public class UserIdentityTests
{
    [Fact]
    public void Constructor_ShouldInitializeEmailAddress()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var user = new UserIdentity(email);

        // Assert
        Assert.Equal(email, user.EmailAddress);
    }

    [Fact]
    public void Constructor_ShouldInitializeEmptyRolesList()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var user = new UserIdentity(email);

        // Assert
        Assert.NotNull(user.Roles);
        Assert.Empty(user.Roles);
    }

    [Fact]
    public void Constructor_WithRole_ShouldInitializeWithSingleRole()
    {
        // Arrange
        var email = "test@example.com";
        var role = new Role("TestRole");

        // Act
        var user = new UserIdentity(email, role);

        // Assert
        Assert.NotNull(user.Roles);
        Assert.Single(user.Roles);
        Assert.Contains(role, user.Roles);
    }
}
