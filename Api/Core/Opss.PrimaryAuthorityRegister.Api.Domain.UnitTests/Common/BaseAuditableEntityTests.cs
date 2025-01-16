using Opss.PrimaryAuthorityRegister.Api.Domain.Common;

namespace Opss.PrimaryAuthorityRegister.Api.Domain.UnitTests.Common;

public class BaseAuditableEntityTests
{
    private class TestAuditableEntity : BaseAuditableEntity
    {
        // Concrete implementation for testing
    }

    [Fact]
    public void Properties_ShouldAllowSettingAndGettingValues()
    {
        // Arrange
        var entity = new TestAuditableEntity();
        var createdDate = DateTime.UtcNow;
        var updatedDate = DateTime.UtcNow.AddHours(1);

        // Act
        entity.CreatedDate = createdDate;
        entity.UpdatedDate = updatedDate;

        // Assert
        Assert.Equal(createdDate, entity.CreatedDate);
        Assert.Equal(updatedDate, entity.UpdatedDate);
    }

    [Fact]
    public void DefaultProperties_ShouldBeNull_WhenInstantiated()
    {
        // Arrange
        var entity = new TestAuditableEntity();

        // Act & Assert
        Assert.Null(entity.CreatedDate);
        Assert.Null(entity.UpdatedDate);
    }
}
