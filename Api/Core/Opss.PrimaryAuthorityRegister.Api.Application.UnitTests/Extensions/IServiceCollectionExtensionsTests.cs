using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Extensions;

public class IServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationLayer_ShouldRegisterMediatR()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationLayer();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verify that MediatR services have been registered
        var mediatorService = serviceProvider.GetService<IMediator>();
        Assert.NotNull(mediatorService);

        // Verify that MediatR has been registered with the assembly from IServiceCollectionExtensions
        var serviceCollection = services.Where(descriptor => descriptor.ServiceType == typeof(IMediator));
        Assert.Contains(serviceCollection, descriptor => descriptor.ImplementationType == typeof(Mediator));
    }
}
