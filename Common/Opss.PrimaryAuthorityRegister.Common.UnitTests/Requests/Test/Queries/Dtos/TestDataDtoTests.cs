using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Queries.Dtos;

namespace Opss.PrimaryAuthorityRegister.Common.UnitTests.Requests.Test.Queries.Dtos;

public class TestDataDtoTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var data = "Sample Data";

        // Act
        var dto = new TestDataDto(id, data);

        // Assert
        Assert.Equal(id, dto.Id);
        Assert.Equal(data, dto.Data);
    }

    [Fact]
    public void Properties_ShouldAllowSettingValues()
    {
        // Arrange
        var dto = new TestDataDto(Guid.Empty, string.Empty);
        var newId = Guid.NewGuid();
        var newData = "Updated Data";

        // Act
        dto.Id = newId;
        dto.Data = newData;

        // Assert
        Assert.Equal(newId, dto.Id);
        Assert.Equal(newData, dto.Data);
    }
}
