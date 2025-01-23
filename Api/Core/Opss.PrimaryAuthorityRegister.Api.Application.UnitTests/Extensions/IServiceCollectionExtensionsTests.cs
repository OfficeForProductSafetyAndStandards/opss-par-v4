using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

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

    [Fact]
    public void AddScoped_ClaimsPrincipal_ReturnsHttpContextUser()
    {
        // Arrange
        var mockHttpContext = new Mock<HttpContext>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }));

        mockHttpContext.Setup(ctx => ctx.User).Returns(claimsPrincipal);
        mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);

        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddScoped(provider =>
        {
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            return httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();
        });

        services.AddSingleton(mockHttpContextAccessor.Object);

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var resolvedClaimsPrincipal = serviceProvider.GetRequiredService<ClaimsPrincipal>();

        // Assert
        Assert.NotNull(resolvedClaimsPrincipal);
        Assert.Equal("TestUser", resolvedClaimsPrincipal.Identity?.Name);
    }
}
