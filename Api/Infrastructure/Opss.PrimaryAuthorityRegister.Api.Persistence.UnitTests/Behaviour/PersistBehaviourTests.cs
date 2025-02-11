using MediatR;
using Moq;
using Opss.PrimaryAuthorityRegister.Api.Application.Interfaces.Repositories;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Behaviour;
using Opss.PrimaryAuthorityRegister.Common.RequestInterfaces;

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.UnitTests.Behaviour;

public class PersistBehaviourTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PersistBehaviour<ICommandBase, string> _persistBehaviour;
    private readonly Mock<RequestHandlerDelegate<string>> _nextMock;

    public PersistBehaviourTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _nextMock = new Mock<RequestHandlerDelegate<string>>();
        _persistBehaviour = new PersistBehaviour<ICommandBase, string>(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_CallsSaveOnSuccessfulExecution()
    {
        // Arrange
        _nextMock.Setup(next => next()).ReturnsAsync("Success");

        // Act
        var result = await _persistBehaviour.Handle(Mock.Of<ICommandBase>(), _nextMock.Object, CancellationToken.None);

        // Assert
        Assert.Equal("Success", result);
        _unitOfWorkMock.Verify(uow => uow.Save(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.Rollback(), Times.Never);
    }

    [Fact]
    public async Task Handle_CallsRollbackOnException()
    {
        // Arrange
        _nextMock.Setup(next => next()).ThrowsAsync(new TimeoutException("Test Exception"));

        // Act & Assert
        await Assert.ThrowsAsync<TimeoutException>(() =>
            _persistBehaviour.Handle(Mock.Of<ICommandBase>(), _nextMock.Object, CancellationToken.None)
        );
        _unitOfWorkMock.Verify(uow => uow.Save(It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Rollback(), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsArgumentNullException_WhenNextIsNull()
    {
        // Act & Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _persistBehaviour.Handle(Mock.Of<ICommandBase>(), null, CancellationToken.None)
        );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        _unitOfWorkMock.Verify(uow => uow.Save(It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.Rollback(), Times.Never);
    }
}