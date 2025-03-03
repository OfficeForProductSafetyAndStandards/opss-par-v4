using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Common.Authority.Queries;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Authority.Queries;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Handlers.Common.Authority.Queries;

public class GetMyOfferedRegulatoryFunctionsQueryHandlerTests
{
    private readonly Mock<IGenericRepository<Domain.Entities.Authority>> _repo;
    private ClaimsPrincipal? _claimsPrincipal;

    public GetMyOfferedRegulatoryFunctionsQueryHandlerTests()
    {
        _repo = new Mock<IGenericRepository<Domain.Entities.Authority>>();
    }

    [Fact]
    public async Task GivenNullClaimsPrincipal_ThenUnauthorizedExceptionThrown()
    {
        // Arrange
        _claimsPrincipal = null;
        var handler = new GetMyOfferedRegulatoryFunctionsQueryHandler(_repo.Object, _claimsPrincipal);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                                handler.Handle(new GetMyOfferedRegulatoryFunctionsQuery(),
                                CancellationToken.None));

        Assert.Equal("You are not authenticated", exception.Message);
    }

    [Fact]
    public async Task GivenNoAuthorityId_ThenUnauthorizedExceptionThrown()
    {
        // Arrange
        _claimsPrincipal = new ClaimsPrincipal();
        var handler = new GetMyOfferedRegulatoryFunctionsQueryHandler(_repo.Object, _claimsPrincipal);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                                handler.Handle(new GetMyOfferedRegulatoryFunctionsQuery(),
                                CancellationToken.None));

        Assert.Equal("You are not assigned to an authority", exception.Message);
    }

    [Fact]
    public async Task GivenNoAuthorityFound_ThenUnauthorizedExceptionThrown()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal();
        claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
        { 
            new Claim(Claims.Authority, Guid.NewGuid().ToString())
        }));

        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .Returns(() => null);
        
        var handler = new GetMyOfferedRegulatoryFunctionsQueryHandler(_repo.Object, claimsPrincipal);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                                handler.Handle(new GetMyOfferedRegulatoryFunctionsQuery(),
                                CancellationToken.None));

        Assert.Equal("Your assigned authority cannot be found", exception.Message);
    }

    [Fact]
    public async Task GivenQuery_ThenDataReturned()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal();
        claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
        {
            new Claim(Claims.Authority, Guid.NewGuid().ToString())
        }));

        var authority = new Mock<Domain.Entities.Authority>("Authority");
        authority.SetupGet(a => a.RegulatoryFunctions)
            .Returns(new List<RegulatoryFunction>
            {
                new RegulatoryFunction("Environmental Health"),
                new RegulatoryFunction("Trading Standards")
            });

        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Domain.Entities.Authority, object>>[]>()))
            .ReturnsAsync(() => authority.Object);

        var handler = new GetMyOfferedRegulatoryFunctionsQueryHandler(_repo.Object, claimsPrincipal);

        // Act
        var result = await handler.Handle(new GetMyOfferedRegulatoryFunctionsQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        var results = result.Select(r => r.Name);

        Assert.Contains("Environmental Health", results);
        Assert.Contains("Trading Standards", results);
    }
}

public class GetMyLocalAuthorityQueryHandlerTests
{
    private readonly Mock<IGenericRepository<Domain.Entities.Authority>> _repo;
    private ClaimsPrincipal? _claimsPrincipal;

    public GetMyLocalAuthorityQueryHandlerTests()
    {
        _repo = new Mock<IGenericRepository<Domain.Entities.Authority>>();
    }

    [Fact]
    public async Task GivenNullClaimsPrincipal_ThenUnauthorizedExceptionThrown()
    {
        // Arrange
        _claimsPrincipal = null;
        var handler = new GetMyLocalAuthorityQueryHandler(_repo.Object, _claimsPrincipal);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                                handler.Handle(new GetMyLocalAuthorityQuery(),
                                CancellationToken.None));

        Assert.Equal("You are not authenticated", exception.Message);
    }

    [Fact]
    public async Task GivenNoAuthorityId_ThenUnauthorizedExceptionThrown()
    {
        // Arrange
        _claimsPrincipal = new ClaimsPrincipal();
        var handler = new GetMyLocalAuthorityQueryHandler(_repo.Object, _claimsPrincipal);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                                handler.Handle(new GetMyLocalAuthorityQuery(),
                                CancellationToken.None));

        Assert.Equal("You are not assigned to an authority", exception.Message);
    }

    [Fact]
    public async Task GivenNoAuthorityFound_ThenUnauthorizedExceptionThrown()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal();
        claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
        {
            new Claim(Claims.Authority, Guid.NewGuid().ToString())
        }));

        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .Returns(() => null);

        var handler = new GetMyLocalAuthorityQueryHandler(_repo.Object, claimsPrincipal);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                                handler.Handle(new GetMyLocalAuthorityQuery(),
                                CancellationToken.None));

        Assert.Equal("Your assigned authority cannot be found", exception.Message);
    }

    [Fact]
    public async Task GivenQuery_ThenDataReturned()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal();
        claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
        {
            new Claim(Claims.Authority, Guid.NewGuid().ToString())
        }));

        var authority = new Domain.Entities.Authority("Authority");

        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Domain.Entities.Authority, object>>[]>()))
            .ReturnsAsync(() => authority);

        var handler = new GetMyLocalAuthorityQueryHandler(_repo.Object, claimsPrincipal);

        // Act
        var result = await handler.Handle(new GetMyLocalAuthorityQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Authority", result.Name);
    }
}

