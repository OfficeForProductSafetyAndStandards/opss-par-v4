using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Common.Authority.Queries;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using Opss.PrimaryAuthorityRegister.Common.Constants;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Common.Authority.Queries;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Cqrs.Requests.PartnershipApplication.Commands;

public class CreatePartnershipApplicationCommandTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private CreatePartnershipApplicationCommandHandler _handler;
    private ClaimsPrincipal? _claimsPrincipal;

    public CreatePartnershipApplicationCommandTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();    
    }

    [Fact]
    public async Task GivenNullCommand_WhenHandlingRequest_ThenArgumentNullExceptionIsThrown()
    {
        // Arrange
        _handler = new CreatePartnershipApplicationCommandHandler(_mockUnitOfWork.Object, _claimsPrincipal);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null, CancellationToken.None));
    }

    [Fact]
    public async Task GivenNullClaimsPrincipal_WhenHandlingRequest_ThenUnauthorizedExceptionThrown()
    {
        // Arrange
        _claimsPrincipal = null;
        _handler = new CreatePartnershipApplicationCommandHandler(_mockUnitOfWork.Object, _claimsPrincipal);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                                _handler.Handle(new CreatePartnershipApplicationCommand(PartnershipConstants.PartnershipType.Direct),
        CancellationToken.None));

        Assert.Equal("You are not authenticated", exception.Message);
    }

    [Fact]
    public async Task GivenNoAuthorityId_WhenHandlingRequest_ThenUnauthorizedExceptionThrown()
    {
        // Arrange
        _claimsPrincipal = new ClaimsPrincipal();
        _handler = new CreatePartnershipApplicationCommandHandler(_mockUnitOfWork.Object, _claimsPrincipal);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                                _handler.Handle(new CreatePartnershipApplicationCommand(PartnershipConstants.PartnershipType.Direct),
                                CancellationToken.None));

        Assert.Equal("You are not assigned to an authority", exception.Message);
    }

    [Fact]
    public async Task GivenNoAuthorityFound_WhenHandlingRequest_ThenUnauthorizedExceptionThrown()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal();
        claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
        {
            new Claim(Claims.Authority, Guid.NewGuid().ToString())
        }));
        _handler = new CreatePartnershipApplicationCommandHandler(_mockUnitOfWork.Object, _claimsPrincipal);

        var authorityRepo = new Mock<IGenericRepository<Authority>>();
        authorityRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Authority, object>>[]>()))
                     .Returns(() => null);

        _mockUnitOfWork.Setup(u => u.Repository<Authority>())
                       .Returns(authorityRepo.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpResponseException>(() =>
                                _handler.Handle(new CreatePartnershipApplicationCommand(PartnershipConstants.PartnershipType.Direct),
                                CancellationToken.None));

        Assert.Equal("Your assigned authority cannot be found", exception.Message);
    }

    [Fact]
    public async Task GivenValidCommand_WhenHandlingRequest_ThenPartnershipApplicationIsCreated()
    {
        // Arrange
        var claimsPrincipal = new ClaimsPrincipal();
        claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
        {
            new Claim(Claims.Authority, Guid.NewGuid().ToString())
        }));
        _handler = new CreatePartnershipApplicationCommandHandler(_mockUnitOfWork.Object, _claimsPrincipal);

        var authorityRepo = new Mock<IGenericRepository<Authority>>();
        var authority = new Authority("Authority");
        authorityRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Authority, object>>[]>()))
            .ReturnsAsync(() => authority);

        _mockUnitOfWork.Setup(u => u.Repository<Authority>())
                       .Returns(authorityRepo.Object);

        var expectedApplication = new Api.Domain.Entities.PartnershipApplication();
        var partnershipApplicationRepo = new Mock<IGenericRepository<Api.Domain.Entities.PartnershipApplication>>();
        partnershipApplicationRepo.Setup(r => r.AddAsync(expectedApplication))
            .ReturnsAsync(expectedApplication);
        _mockUnitOfWork.Setup(u => u.Repository<Api.Domain.Entities.PartnershipApplication>())
                       .Returns(partnershipApplicationRepo.Object);

        var command = new CreatePartnershipApplicationCommand(PartnershipConstants.PartnershipType.Direct);
        var response = await _handler.Handle(command, CancellationToken.None);

        _mockUnitOfWork.Verify(u => u.Save(It.IsAny<CancellationToken>()), Times.Once);
        partnershipApplicationRepo.Verify(r => r.AddAsync(expectedApplication), Times.Once);

        Assert.Equal(expectedApplication.Id, response);
    }
}
