using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Exceptions;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Extensions;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.UnitTests.Extensions;

public class IApplicationBuilderExtensionsTests
{
    [Fact]
    public void MigrateDatabase_ShouldThrowArgumentNullException_WhenAppIsNull()
    {
        // Arrange
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        IApplicationBuilder app = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Act & Assert
#pragma warning disable CS8604 // Possible null reference argument.
        Assert.Throws<ArgumentNullException>(() => app.MigrateDatabase());
#pragma warning restore CS8604 // Possible null reference argument.
    }

    [Fact]
    public void MigrateDatabase_ShouldThrowServiceNotFoundException_WhenServiceScopeFactoryIsMissing()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        var mockAppBuilder = new Mock<IApplicationBuilder>();
        mockAppBuilder.Setup(a => a.ApplicationServices).Returns(mockServiceProvider.Object);

        // Act & Assert
        var exception = Assert.Throws<ServiceNotFoundException>(() => mockAppBuilder.Object.MigrateDatabase());
        Assert.Equal($"Service: {nameof(IServiceScopeFactory)} not found", exception.Message);
    }
}

