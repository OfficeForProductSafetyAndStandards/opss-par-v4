using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;

namespace Opss.PrimaryAuthorityRegister.Cqrs.UnitTests.AuthorisationAttributes;

public class MustHaveRoleAttributeTests
{
    [Fact]
    public void MustHaveRoleAttribute_ReturnsPermission_WhenSupplied()
    {
        // Arrange
        var roleRequiredAttribute =
                typeof(SampleResource).GetCustomAttributes(typeof(MustHaveRoleAttribute), false).Cast
                        <MustHaveRoleAttribute>().Single();

        var role = roleRequiredAttribute.Role;
        var roles = roleRequiredAttribute.Roles;

        Assert.Equal("Admin", role);
        Assert.Contains("Admin", roles);
    }

    [MustHaveRole("Admin")]
    private sealed class SampleResource { }

    [Fact]
    public void MustHaveRoleAttribute_ReturnsPermissions_WhenMultipleSupplied()
    {
        // Arrange
        var roleRequiredAttribute =
                typeof(DoulbeResource).GetCustomAttributes(typeof(MustHaveRoleAttribute), false).Cast
                        <MustHaveRoleAttribute>().Single();

        var role = roleRequiredAttribute.Role;
        var roles = roleRequiredAttribute.Roles;

        Assert.Equal("Admin|Member", role);
        Assert.Contains("Admin", roles);
        Assert.Contains("Member", roles);
    }

    [MustHaveRole("Admin", "Member")]
    private sealed class DoulbeResource { }
}
