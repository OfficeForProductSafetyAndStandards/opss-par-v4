using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Test;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Commands;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Handlers.Test;

public class CreateTestDataCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<IGenericRepository<TestData>> repositoryMock;
    private readonly CreateTestDataCommandHandler handler;

    public CreateTestDataCommandHandlerTests()
    {
        unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock = new Mock<IGenericRepository<TestData>>();
        unitOfWorkMock.Setup(uow => uow.Repository<TestData>()).Returns(repositoryMock.Object);
        handler = new CreateTestDataCommandHandler(unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        CreateTestDataCommand request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallAddAsyncWithCorrectData_WhenRequestIsValid()
    {
        // Arrange
        var testData = new TestData("Sample Data");
        var expectedId = Guid.NewGuid();

        repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<TestData>()))
            .ReturnsAsync(new TestData("Sample Data") { Id = expectedId });

        var request = new CreateTestDataCommand(testData.Data);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        repositoryMock.Verify(repo => repo.AddAsync(It.Is<TestData>(d => d.Data == request.Data)), Times.Once);
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task Handle_ShouldReturnGeneratedId_WhenRequestIsValid()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var testData = new TestData("Sample Data") { Id = expectedId };

        repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<TestData>()))
            .ReturnsAsync(testData);

        var request = new CreateTestDataCommand("Sample Data");

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
    }
}

