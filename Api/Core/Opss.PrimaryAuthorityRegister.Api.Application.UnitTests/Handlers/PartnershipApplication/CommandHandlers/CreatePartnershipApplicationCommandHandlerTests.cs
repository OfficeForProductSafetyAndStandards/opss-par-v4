using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.PartnershipApplication.CommandHandlers;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Authentication.Constants;
using Opss.PrimaryAuthorityRegister.Common.Constants;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.PartnershipApplication.Commands;
using Opss.PrimaryAuthorityRegister.Http.Exceptions;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Handlers.PartnershipApplication.CommandHandlers;

public class CreatePartnershipApplicationCommandHandlerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private CreatePartnershipApplicationCommandHandler _handler;
    private ClaimsPrincipal? _claimsPrincipal;

    public CreatePartnershipApplicationCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
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
        _claimsPrincipal = new ClaimsPrincipal();
        _claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
        {
            new Claim(Claims.Authority, Guid.NewGuid().ToString())
        }));
        _handler = new CreatePartnershipApplicationCommandHandler(_mockUnitOfWork.Object, _claimsPrincipal);

        var authorityRepo = new Mock<IGenericRepository<Authority>>();
        authorityRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync(() => null);

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
        var authorityId = Guid.NewGuid();
        _claimsPrincipal = new ClaimsPrincipal();
        _claimsPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>
        {
            new Claim(Claims.Authority, authorityId.ToString())
        }));
        _handler = new CreatePartnershipApplicationCommandHandler(_mockUnitOfWork.Object, _claimsPrincipal);

        var authorityRepo = new Mock<IGenericRepository<Authority>>();
        var authority = new Authority("Authority");
        authorityRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => authority);

        _mockUnitOfWork.Setup(u => u.Repository<Authority>())
                       .Returns(authorityRepo.Object);

        var expectedApplication = new Domain.Entities.PartnershipApplication(
            authorityId, 
            PartnershipConstants.PartnershipType.Direct);
        var partnershipApplicationRepo = new Mock<IGenericRepository<Domain.Entities.PartnershipApplication>>();
        partnershipApplicationRepo.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.PartnershipApplication>()))
            .ReturnsAsync(expectedApplication);
        _mockUnitOfWork.Setup(u => u.Repository<Domain.Entities.PartnershipApplication>())
                       .Returns(partnershipApplicationRepo.Object);

        var command = new CreatePartnershipApplicationCommand(PartnershipConstants.PartnershipType.Direct);
        var response = await _handler.Handle(command, CancellationToken.None);

        partnershipApplicationRepo.Verify(
            repo => repo.AddAsync(
                It.Is<Domain.Entities.PartnershipApplication>(
                    d => d.PartnershipType == command.PartnershipType)), Times.Once);

        Assert.Equal(expectedApplication.Id, response);
    }
}

