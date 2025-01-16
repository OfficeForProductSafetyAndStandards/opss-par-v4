using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Test;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Handlers.Test;

public class UpdateTestDataCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<IGenericRepository<TestData>> repositoryMock;
    private readonly UpdateTestDataCommandHandler handler;

    public UpdateTestDataCommandHandlerTests()
    {
        unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock = new Mock<IGenericRepository<TestData>>();
        unitOfWorkMock.Setup(uow => uow.Repository<TestData>()).Returns(repositoryMock.Object);
        handler = new UpdateTestDataCommandHandler(unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        UpdateTestDataCommand request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenDataIsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        repositoryMock
            .Setup(repo => repo.GetByIdAsync(nonExistentId))
            .ReturnsAsync((TestData)null);

        var request = new UpdateTestDataCommand(nonExistentId, "Updated Data");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(request, CancellationToken.None));
        Assert.Equal($"No data found with Id {nonExistentId}", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldCallUpdateAsyncWithUpdatedData_WhenRequestIsValid()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var existingData = new TestData("Existing Data") { Id = existingId };

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(existingId))
            .ReturnsAsync(existingData);

        repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<TestData>()))
            .Returns(Task.CompletedTask);

        var request = new UpdateTestDataCommand(existingId, "Updated Data");

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        repositoryMock.Verify(repo => repo.GetByIdAsync(existingId), Times.Once);
        repositoryMock.Verify(repo => repo.UpdateAsync(It.Is<TestData>(d => d.Id == existingId && d.Data == request.Data)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateDataCorrectly_WhenRequestIsValid()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var existingData = new TestData("Old Data") { Id = existingId };

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(existingId))
            .ReturnsAsync(existingData);

        repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<TestData>()))
            .Returns(Task.CompletedTask);

        var request = new UpdateTestDataCommand(existingId,"New Data");

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(request.Data, existingData.Data);
    }
}
