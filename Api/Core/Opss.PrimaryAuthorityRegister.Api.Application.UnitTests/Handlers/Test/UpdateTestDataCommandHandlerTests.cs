using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Test;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Handlers.Test;

public class UpdateTestDataCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<TestData>> _repositoryMock;
    private readonly UpdateTestDataCommandHandler _handler;

    public UpdateTestDataCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IGenericRepository<TestData>>();
        _unitOfWorkMock.Setup(uow => uow.Repository<TestData>()).Returns(_repositoryMock.Object);
        _handler = new UpdateTestDataCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        UpdateTestDataCommand request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenDataIsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(nonExistentId))
            .ReturnsAsync((TestData)null);

        var ownerId = Guid.NewGuid();
        var request = new UpdateTestDataCommand(ownerId, nonExistentId, "Updated Data");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(request, CancellationToken.None));
        Assert.Equal($"No data found with Id {nonExistentId}", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldCallUpdateAsyncWithUpdatedData_WhenRequestIsValid()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var existingId = Guid.NewGuid();
        var existingData = new TestData(ownerId, "Existing Data") { Id = existingId };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(existingId))
            .ReturnsAsync(existingData);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<TestData>()))
            .Returns(Task.CompletedTask);

        var request = new UpdateTestDataCommand(ownerId, existingId, "Updated Data");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(repo => repo.GetByIdAsync(existingId), Times.Once);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.Is<TestData>(d => d.Id == existingId && d.Data == request.Data)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateDataCorrectly_WhenRequestIsValid()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var existingId = Guid.NewGuid();
        var existingData = new TestData(ownerId, "Old Data") { Id = existingId };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(existingId))
            .ReturnsAsync(existingData);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<TestData>()))
            .Returns(Task.CompletedTask);

        var request = new UpdateTestDataCommand(ownerId, existingId,"New Data");

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(request.Data, existingData.Data);
    }
}
