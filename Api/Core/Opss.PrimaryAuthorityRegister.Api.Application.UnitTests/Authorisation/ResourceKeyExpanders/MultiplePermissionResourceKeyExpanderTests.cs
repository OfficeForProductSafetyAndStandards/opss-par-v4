using Opss.PrimaryAuthorityRegister.Api.Application.Authorisation.ResourceKeyExpanders;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Authorisation.ResourceKeyExpanders;

public class MultiplePermissionResourceKeyExpanderTests
{
    [Fact]
    public void WhenExpandingMultiplePermissions_PermissionsAreExpanded()
    {
        // Arrange
        var expander = new MultiplePermissionResourceKeyExpander();
        var permissions = "Admin|User";

        // Act
        var expanded = expander.GetKeys(permissions);

        // Assert
        Assert.Equal(permissions.Split("|"), expanded);
    }
}
