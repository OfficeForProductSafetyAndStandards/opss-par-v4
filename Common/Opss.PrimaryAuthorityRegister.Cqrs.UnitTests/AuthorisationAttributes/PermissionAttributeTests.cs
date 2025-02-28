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

        Assert.Equal("Admin", permission);
    }

    [Permission("Admin")]
    private sealed class SampleResource { }
}
