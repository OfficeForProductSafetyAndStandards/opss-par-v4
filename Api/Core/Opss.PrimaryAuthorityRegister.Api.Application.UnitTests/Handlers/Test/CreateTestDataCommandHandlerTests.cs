using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Test;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Cqrs.Requests.Test.Commands;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Handlers.Test;

public class CreateTestDataCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<TestData>> _repositoryMock;
    private readonly CreateTestDataCommandHandler _handler;

    public CreateTestDataCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IGenericRepository<TestData>>();
        _unitOfWorkMock.Setup(uow => uow.Repository<TestData>()).Returns(_repositoryMock.Object);
        _handler = new CreateTestDataCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        CreateTestDataCommand request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallAddAsyncWithCorrectData_WhenRequestIsValid()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var testData = new TestData(ownerId, "Sample Data");
        var expectedId = Guid.NewGuid();

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<TestData>()))
            .ReturnsAsync(new TestData(ownerId, "Sample Data") { Id = expectedId });

        var request = new CreateTestDataCommand(ownerId, testData.Data);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(repo => repo.AddAsync(It.Is<TestData>(d => d.Data == request.Data)), Times.Once);
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task Handle_ShouldReturnGeneratedId_WhenRequestIsValid()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var expectedId = Guid.NewGuid();
        var testData = new TestData(ownerId, "Sample Data") { Id = expectedId };

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<TestData>()))
            .ReturnsAsync(testData);

        var request = new CreateTestDataCommand(ownerId, "Sample Data");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
    }
}

