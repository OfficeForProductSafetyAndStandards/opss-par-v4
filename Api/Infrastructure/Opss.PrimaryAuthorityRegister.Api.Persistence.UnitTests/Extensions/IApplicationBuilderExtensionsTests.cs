﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Extensions;
using Opss.PrimaryAuthorityRegister.Common.Exceptions;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.UnitTests.Extensions;

public class IApplicationBuilderExtensionsTests
{
    [Fact]
    public void MigrateDatabase_ShouldThrowArgumentNullException_WhenAppIsNull()
    {
        // Arrange
        IApplicationBuilder app = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => app.MigrateDatabase());
    }

    [Fact]
    public void MigrateDatabase_ShouldThrowServiceNotFoundException_WhenServiceScopeFactoryIsMissing()
    {
        // Arrange
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(null);

        var mockAppBuilder = new Mock<IApplicationBuilder>();
        mockAppBuilder.Setup(a => a.ApplicationServices).Returns(mockServiceProvider.Object);

        // Act & Assert
        var exception = Assert.Throws<ServiceNotFoundException>(() => mockAppBuilder.Object.MigrateDatabase());
        Assert.Equal($"Service: {nameof(IServiceScopeFactory)} not found", exception.Message);
    }
}

