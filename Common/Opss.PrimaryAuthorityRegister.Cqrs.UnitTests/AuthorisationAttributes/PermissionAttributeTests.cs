using Opss.PrimaryAuthorityRegister.Cqrs.AuthorisationAttributes;

namespace Opss.PrimaryAuthorityRegister.Cqrs.UnitTests.AuthorisationAttributes;

public class PermissionAttributeTests
{
    [Fact]
    public void PermissionAttribute_ReturnsPermission_WhenSupplied()
    {
        // Arrange
        var claimRequiredAttribute =
                typeof(SampleResource).GetCustomAttributes(typeof(PermissionAttribute), false).Cast
                        <PermissionAttribute>().Single();

        var permission = claimRequiredAttribute.Permission;
        var permissions = claimRequiredAttribute.Permissions;

        Assert.Equal("Admin", permission);
        Assert.Contains("Admin", permissions);
    }

    [Permission("Admin")]
    private sealed class SampleResource { }

    [Fact]
    public void PermissionAttribute_ReturnsPermissions_WhenMultipleSupplied()
    {
        // Arrange
        var claimRequiredAttribute =
                typeof(DoulbeResource).GetCustomAttributes(typeof(PermissionAttribute), false).Cast
                        <PermissionAttribute>().Single();

        var permission = claimRequiredAttribute.Permission;
        var permissions = claimRequiredAttribute.Permissions;

        Assert.Equal("Admin|Member", permission);
        Assert.Contains("Admin", permissions);
        Assert.Contains("Member", permissions);
    }

    [Permission("Admin", "Member")]
    private sealed class DoulbeResource { }
}
public class PermissionForAttributeTests
{
    [Fact]
    public void PermissionForAttribute_ReturnsPermission_WhenSupplied()
    {
        // Arrange
        var claimRequiredAttribute =
                typeof(SampleResource).GetCustomAttributes(typeof(PermissionForAttribute), false).Cast
                        <PermissionForAttribute>().Single();

        var permission = claimRequiredAttribute.Permission;
        var template = claimRequiredAttribute.ResourceKeyTemplate;

        Assert.Equal("Admin", permission);
        Assert.Equal("Object/Id", template);
    }

    [PermissionFor("Admin", "Object/Id")]
    private sealed class SampleResource { }
}
