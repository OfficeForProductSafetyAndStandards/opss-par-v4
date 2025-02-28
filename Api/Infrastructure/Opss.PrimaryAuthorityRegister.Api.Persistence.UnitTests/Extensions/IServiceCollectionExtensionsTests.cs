using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Moq;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Contexts;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Extensions;
using Opss.PrimaryAuthorityRegister.Common.Providers;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.UnitTests.Extensions;

public class IServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPersistenceLayer_ShouldAddDbContext_WhenValidConfigurationProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IDateTimeProvider, DateTimeOverrideProvider>();
        
        // Create a mock IConfiguration to return the connection string
        var configurationMock = new Mock<IConfiguration>();
        var connectionStringsMock = new Mock<IConfigurationSection>();
        connectionStringsMock.Setup(x => x["DefaultConnection"]).Returns("ValidConnectionString");
        configurationMock.Setup(x => x.GetSection("ConnectionStrings")).Returns(connectionStringsMock.Object);

        // Act
        services.AddPersistenceLayer(configurationMock.Object);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<ApplicationDbContext>();
        Assert.NotNull(dbContext);  // Check if dbContext is added correctly.
    }

    [Fact]
    public void AddDbContext_ShouldThrowException_WhenConnectionStringIsMissing()
    {
        // Arrange
        var services = new ServiceCollection();

        // Create a mock IConfiguration to return null for the connection string
        var configurationMock = new Mock<IConfiguration>();
        var connectionStringsMock = new Mock<IConfigurationSection>();
        connectionStringsMock.Setup(x => x["DefaultConnection"]).Returns((string?)null);
        configurationMock.Setup(x => x.GetSection("ConnectionStrings")).Returns(connectionStringsMock.Object);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddDbContext(configurationMock.Object));
        Assert.Equal("Configuration missing value for DefaultConnection connection string", exception.Message);
    }
}

