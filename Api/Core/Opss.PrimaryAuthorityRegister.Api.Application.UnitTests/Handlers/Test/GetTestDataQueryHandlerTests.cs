using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Handlers.Test;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Domain.Entities;
using Opss.PrimaryAuthorityRegister.Common.Requests.Test.Queries;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Handlers.Test;

public class GetTestDataQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<IGenericRepository<TestData>> repositoryMock;
    private readonly GetTestDataQueryHandler handler;

    public GetTestDataQueryHandlerTests()
    {
        unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock = new Mock<IGenericRepository<TestData>>();
        unitOfWorkMock.Setup(uow => uow.Repository<TestData>()).Returns(repositoryMock.Object);
        handler = new GetTestDataQueryHandler(unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        GetTestDataQuery request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallGetByIdAsyncWithCorrectId_WhenRequestIsValid()
    {
        // Arrange
        var testData = new TestData("Sample Data") { Id = Guid.NewGuid() };
        repositoryMock
            .Setup(repo => repo.GetByIdAsync(testData.Id))
            .ReturnsAsync(testData);

        var request = new GetTestDataQuery(testData.Id);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        repositoryMock.Verify(repo => repo.GetByIdAsync(testData.Id), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(testData.Id, result.Id);
        Assert.Equal(testData.Data, result.Data);
    }

    [Fact]
    public async Task Handle_ShouldReturnTestDataDto_WhenDataIsFound()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var expectedData = "Sample Data";
        var testData = new TestData(expectedData) { Id = expectedId };

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(expectedId))
            .ReturnsAsync(testData);

        var request = new GetTestDataQuery (expectedId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedId, result.Id);
        Assert.Equal(expectedData, result.Data);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenDataIsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        repositoryMock
            .Setup(repo => repo.GetByIdAsync(nonExistentId))
            .ReturnsAsync((TestData)null);

        var request = new GetTestDataQuery (nonExistentId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(request, CancellationToken.None));
        Assert.Equal($"No data found with Id {nonExistentId}", exception.Message);
    }
}
