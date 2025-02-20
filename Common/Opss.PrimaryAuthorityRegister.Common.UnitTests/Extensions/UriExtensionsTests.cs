using Opss.PrimaryAuthorityRegister.Common.ExtensionMethods;

namespace Opss.PrimaryAuthorityRegister.Common.UnitTests.Extensions;

public class UriExtensionsTests
{
    [Fact]
    public void AppendPath_ShouldAppendPathToUri()
    {
        // Arrange
        var baseUri = new Uri("https://example.com");
        var path = "test/path";

        // Act
        var result = baseUri.AppendPath(path);

        // Assert
        Assert.Equal("https://example.com/test/path", result.ToString());
    }

    [Fact]
    public void AppendPath_ShouldHandleTrailingAndLeadingSlashes()
    {
        // Arrange
        var baseUri = new Uri("https://example.com/");
        var path = "/test/path/";

        // Act
        var result = baseUri.AppendPath(path);

        // Assert
        Assert.Equal("https://example.com/test/path/", result.ToString());
    }

    [Fact]
    public void AppendPath_ShouldThrowArgumentNullException_WhenUriIsNull()
    {
        // Arrange
        Uri baseUri = null;
        var path = "test";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => baseUri.AppendPath(path));
    }

    [Fact]
    public void AppendPath_ShouldThrowArgumentNullException_WhenPathIsNull()
    {
        // Arrange
        var baseUri = new Uri("https://example.com");
        string path = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => baseUri.AppendPath(path));
    }
}
